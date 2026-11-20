using HybridExamples;
using OptimizationIntegrationSystem.zhenghao;

return Input.Run(() =>
{
    var data = Input.Read<DynamicInput>(args);
    var planner = new Planner(data);
    var accepted = data.Accepted.ToList();
    var oldPlan = planner.Solve(data.Depot, 0, accepted, [], []) ?? throw new ArgumentException("Initial accepted orders are infeasible.");
    var frozen = oldPlan.Steps.TakeWhile(x => x.Start < data.Now).ToArray();
    if (frozen.Length > 0 && frozen[^1].End < data.Now)
        frozen = frozen.Append(new Step("wait", frozen[^1].To, frozen[^1].To, frozen[^1].End, data.Now)).ToArray();
    string anchorSite = frozen.Length == 0 ? data.Depot : frozen[^1].To;
    int anchorTime = Math.Max(data.Now, frozen.Length == 0 ? 0 : frozen[^1].End);
    var picked = frozen.Where(x => x.Kind == "pickup").Select(x => x.Order!).ToHashSet();
    var delivered = frozen.Where(x => x.Kind == "delivery").Select(x => x.Order!).ToHashSet();
    var suffix = planner.Solve(anchorSite, anchorTime, accepted, picked, delivered)
        ?? throw new InvalidOperationException("Frozen prefix has no feasible continuation.");
    Console.WriteLine($"At t={data.Now}: frozenSteps={frozen.Length}; replanFrom={anchorSite}@{anchorTime}; onboard={string.Join(',', picked.Except(delivered))}");

    int acceptCount = 0, rejectCount = 0;
    foreach (var incoming in data.Incoming)
    {
        Input.Require(incoming.Release <= data.Now, "Future order cannot be processed before release: " + incoming.Id);
        var candidateOrders = accepted.Append(incoming).ToList();
        var candidate = planner.Solve(anchorSite, anchorTime, candidateOrders, picked, delivered);
        if (candidate is null)
        {
            rejectCount++;
            Console.WriteLine($"REJECT {incoming.Id}: no feasible suffix within its time window");
        }
        else
        {
            acceptCount++;
            accepted.Add(incoming);
            suffix = candidate;
            Console.WriteLine($"ACCEPT {incoming.Id}: suffixTravel={candidate.Cost:F2}; returnTime={candidate.End}");
        }
    }
    var updated = frozen.Concat(suffix.Steps).ToArray();
    planner.CheckPlan(updated, accepted);
    Input.Assert(updated.Take(frozen.Length).SequenceEqual(frozen), "committed steps must remain unchanged records");
    Input.Assert(updated.Where(x => x.Kind == "pickup").Select(x => x.Order).ToHashSet().SetEquals(accepted.Select(x => x.Id)), "each accepted order picked once");
    Input.Assert(updated.Where(x => x.Kind == "delivery").Select(x => x.Order).ToHashSet().SetEquals(accepted.Select(x => x.Id)), "each accepted order delivered");
    Input.Assert(updated.Where(x => x.Kind == "pickup").Count() == accepted.Count && updated.Where(x => x.Kind == "delivery").Count() == accepted.Count, "one pickup and delivery per accepted order");
    Input.Assert(acceptCount == 1 && rejectCount == 1, "synthetic batch should accept r2 and reject r3");
    foreach (var step in updated)
    {
        ArcType kind = step.Kind;
        ArcTime time = $"{step.Start}:{step.End}";
        Console.WriteLine($"{time} {kind} {step.From}->{step.To} {step.Order ?? ""}".TrimEnd());
    }
    // t=1 lies inside the planned travel arc [0,2]; commit that whole arc.
    var inProgress = oldPlan.Steps.TakeWhile(x => x.Start < 1).ToArray();
    Input.Assert(inProgress[^1].End == 2, "in-progress travel must finish before replanning");
    Input.Assert(frozen.Take(Math.Min(frozen.Length, oldPlan.Steps.Length)).SequenceEqual(oldPlan.Steps.Take(frozen.Length)), "old plan remains unchanged");
    Console.WriteLine("PASS: frozen prefix; onboard state; accepted/rejected updates; full service; in-progress commitment.");
});

sealed record Order(string Id, string Pickup, string Delivery, int Release, int LatestDelivery, int Quantity);
sealed record Link(string From, string To, int Duration, double Cost);
sealed record DynamicInput(string Depot, int Capacity, int Now, int Horizon, int ServiceTime, Link[] Links, Order[] Accepted, Order[] Incoming);
sealed record Step(string Kind, string From, string To, int Start, int End, string? Order = null);
sealed record Plan(Step[] Steps, double Cost, int End);

sealed class Planner
{
    readonly DynamicInput data;
    readonly Dictionary<(string, string), Link> links;
    readonly HashSet<string> sites;

    public Planner(DynamicInput data)
    {
        Input.Require(data.Capacity > 0 && data.Now >= 0 && data.Horizon > data.Now && data.ServiceTime > 0, "invalid planning horizon/capacity/service time");
        Input.Require(data.Links is { Length: > 0 } && data.Accepted is { Length: > 0 } && data.Incoming is { Length: > 0 }, "links/orders are required");
        Input.Require(data.Links.Select(x => (x.From, x.To)).Distinct().Count() == data.Links.Length, "duplicate directed link");
        this.data = data;
        links = data.Links.ToDictionary(x => (x.From, x.To));
        sites = data.Links.SelectMany(x => new[] { x.From, x.To }).ToHashSet();
        Input.Require(sites.Contains(data.Depot), "depot is absent");
        foreach (var link in data.Links)
            Input.Require(link.Duration > 0 && Input.FiniteNonnegative(link.Cost) && link.From != link.To, "invalid directed link");
        // Require a direct travel link between each pair of distinct locations.
        foreach (var a in sites) foreach (var b in sites)
            Input.Require(a == b || links.ContainsKey((a, b)), "missing direct travel link " + a + "->" + b);
        var orders = data.Accepted.Concat(data.Incoming).ToArray();
        Input.Require(orders.Select(x => x.Id).Distinct().Count() == orders.Length, "duplicate order id");
        foreach (var order in orders)
            Input.Require(order.Quantity > 0 && order.Release >= 0 && order.LatestDelivery >= order.Release && sites.Contains(order.Pickup) && sites.Contains(order.Delivery), "invalid order " + order.Id);
    }

    public Plan? Solve(string start, int time, IReadOnlyList<Order> orders, HashSet<string> picked, HashSet<string> delivered)
    {
        // Enumerate pickup/delivery sequences that respect precedence.
        // State is copied at each branch so a rejected candidate cannot mutate a plan.
        Plan? best = null;
        void Search(string site, int at, HashSet<string> hasPicked, HashSet<string> hasDelivered, List<Step> path, double cost)
        {
            if (at > data.Horizon || (best is not null && cost > best.Cost)) return;
            if (hasDelivered.Count == orders.Count)
            {
                var (duration, travelCost) = Travel(site, data.Depot);
                if (at + duration > data.Horizon) return;
                var endPath = new List<Step>(path);
                if (duration > 0) endPath.Add(new("move", site, data.Depot, at, at + duration));
                var plan = new Plan(endPath.ToArray(), cost + travelCost, at + duration);
                if (best is null || plan.Cost < best.Cost || (plan.Cost == best.Cost && plan.End < best.End)) best = plan;
                return;
            }
            int load = orders.Where(o => hasPicked.Contains(o.Id) && !hasDelivered.Contains(o.Id)).Sum(o => o.Quantity);
            foreach (var order in orders)
            {
                if (hasDelivered.Contains(order.Id)) continue;
                bool pickup = !hasPicked.Contains(order.Id);
                if (pickup && load + order.Quantity > data.Capacity) continue;
                string nextSite = pickup ? order.Pickup : order.Delivery;
                var (duration, travelCost) = Travel(site, nextSite);
                int arrive = at + duration;
                int begin = pickup ? Math.Max(arrive, order.Release) : arrive;
                int end = begin + data.ServiceTime;
                if (end > order.LatestDelivery || end > data.Horizon) continue;
                var nextPath = new List<Step>(path);
                if (duration > 0) nextPath.Add(new("move", site, nextSite, at, arrive));
                if (begin > arrive) nextPath.Add(new("wait", nextSite, nextSite, arrive, begin));
                nextPath.Add(new(pickup ? "pickup" : "delivery", nextSite, nextSite, begin, end, order.Id));
                var nextPicked = new HashSet<string>(hasPicked);
                var nextDelivered = new HashSet<string>(hasDelivered);
                if (pickup) nextPicked.Add(order.Id); else nextDelivered.Add(order.Id);
                Search(nextSite, end, nextPicked, nextDelivered, nextPath, cost + travelCost);
            }
        }
        Search(start, time, new HashSet<string>(picked), new HashSet<string>(delivered), [], 0);
        return best;
    }

    public void CheckPlan(Step[] steps, IReadOnlyList<Order> accepted)
    {
        string site = data.Depot;
        int time = 0, load = 0;
        var pickup = new HashSet<string>();
        var delivery = new HashSet<string>();
        foreach (var step in steps)
        {
            Input.Assert(step.From == site && step.Start == time && step.End > step.Start, "route continuity");
            if (step.Kind == "move")
                Input.Assert(links[(step.From, step.To)].Duration == step.End - step.Start, "travel duration");
            else
            {
                Input.Assert(step.From == step.To, "service/wait location");
                if (step.Kind is "pickup" or "delivery")
                {
                    var order = accepted.Single(o => o.Id == step.Order);
                    Input.Assert(step.End - step.Start == data.ServiceTime, "service duration");
                    if (step.Kind == "pickup")
                    {
                        Input.Assert(order.Pickup == site && step.Start >= order.Release && pickup.Add(order.Id), "pickup once after release");
                        load += order.Quantity;
                    }
                    else
                    {
                        Input.Assert(order.Delivery == site && step.End <= order.LatestDelivery && pickup.Contains(order.Id) && delivery.Add(order.Id), "delivery precedence/deadline");
                        load -= order.Quantity;
                    }
                }
            }
            Input.Assert(load >= 0 && load <= data.Capacity, "onboard capacity");
            site = step.To;
            time = step.End;
        }
        Input.Assert(site == data.Depot && time <= data.Horizon && load == 0 && delivery.Count == accepted.Count, "completed plan");
    }

    (int duration, double cost) Travel(string from, string to)
    {
        if (from == to) return (0, 0);
        var link = links[(from, to)];
        return (link.Duration, link.Cost);
    }
}
