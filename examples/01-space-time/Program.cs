using HybridExamples;
using OptimizationIntegrationSystem.zhenghao;

return Input.Run(() =>
{
    var data = Input.Read<NetworkInput>(args);
    var network = new Network(data);
    foreach (var route in data.Routes)
    {
        var result = network.Validate(route);
        Input.Assert(result.Valid, route.Vehicle + ": " + result.Reason);
        Console.WriteLine($"{route.Vehicle}: feasible; arcs={route.Arcs.Length}; cost={result.Cost:F2}; peakLoad={result.PeakLoad}");
    }
    var first = data.Routes[0];
    Input.Assert(!network.Validate(first with { Capacity = 0 }).Valid, "capacity violation must fail");
    Input.Assert(!network.Validate(first with { Arcs = first.Arcs.Skip(1).ToArray() }).Valid, "broken source must fail");
    var late = data with { Orders = data.Orders.Select(o => o.Id == "r1" ? o with { LatestDelivery = 5 } : o).ToArray() };
    Input.Assert(!new Network(late).Validate(first).Valid, "late delivery must fail");
    var wrongLayer = first with { Layer = "robot" };
    Input.Assert(!network.Validate(wrongLayer).Valid, "vehicle layer mismatch must fail");
    // Check equality for the shared ArcTime and ArcType value objects.
    Input.Assert(new ArcTime("0:1") == new ArcTime("0:1"), "original ArcTime equality");
    Input.Assert(new ArcType("move").Equals(new ArcType("move")), "original ArcType equality");
    Console.WriteLine($"PASS: {data.Arcs.Length} directed time-space arcs; 2 feasible routes; 4 infeasible variants; original ArcTime/ArcType checks.");
});

sealed record Vertex(string Site, string State, int Time);
sealed record Edge(string Id, string Layer, string Kind, Vertex From, Vertex To, double Cost, string? Order);
sealed record Order(string Id, string Pickup, string Delivery, int Release, int EarliestPickup, int LatestDelivery, int Quantity, bool Exclusive);
sealed record Route(string Vehicle, string Layer, int Capacity, Vertex Start, Vertex End, string[] Arcs);
sealed record NetworkInput(Order[] Orders, Edge[] Arcs, Route[] Routes);
sealed record Feasibility(bool Valid, string Reason, double Cost = 0, int PeakLoad = 0);

sealed class Network
{
    readonly Dictionary<string, Edge> edges;
    readonly Dictionary<string, Order> orders;

    public Network(NetworkInput data)
    {
        Input.Require(data.Orders is { Length: > 0 } && data.Arcs is { Length: > 0 } && data.Routes is { Length: > 0 }, "orders, arcs and routes are required");
        Input.Require(data.Orders.Select(x => x.Id).Distinct().Count() == data.Orders.Length, "duplicate order id");
        Input.Require(data.Arcs.Select(x => x.Id).Distinct().Count() == data.Arcs.Length, "duplicate arc id");
        orders = data.Orders.ToDictionary(x => x.Id);
        edges = data.Arcs.ToDictionary(x => x.Id);
        foreach (var order in orders.Values)
            Input.Require(order.Quantity > 0 && order.Release >= 0 && order.EarliestPickup >= order.Release && order.LatestDelivery >= order.EarliestPickup,
                "invalid quantity or time window for " + order.Id);
        foreach (var edge in edges.Values)
        {
            Input.Require(edge.From is not null && edge.To is not null && edge.From.Time >= 0 && edge.To.Time > edge.From.Time, "arcs must advance time: " + edge.Id);
            Input.Require(Input.FiniteNonnegative(edge.Cost), "invalid arc cost: " + edge.Id);
            Input.Require(edge.Layer is "truck" or "robot" && edge.Kind is "move" or "wait" or "pickup" or "delivery", "unknown arc layer or kind");
            Input.Require(edge.Kind == "move" || edge.From.Site == edge.To.Site, "non-travel arc changes location");
            if (edge.Kind is "pickup" or "delivery") Input.Require(edge.Order is not null && orders.ContainsKey(edge.Order), "unknown service order");
            else Input.Require(edge.Order is null, "move/wait must not claim service");
        }
    }

    public Feasibility Validate(Route route)
    {
        if (route.Capacity <= 0 || route.Arcs.Length == 0) return new(false, "empty route or nonpositive capacity");
        var current = route.Start;
        var onboard = new HashSet<string>();
        var picked = new HashSet<string>();
        double cost = 0;
        int peak = 0;
        foreach (var id in route.Arcs)
        {
            if (!edges.TryGetValue(id, out var edge)) return new(false, "unknown arc " + id);
            if (edge.Layer != route.Layer || edge.From != current) return new(false, "layer or endpoint mismatch " + id);
            if (edge.Kind == "pickup")
            {
                var order = orders[edge.Order!];
                if (edge.From.Site != order.Pickup || edge.From.Time < order.EarliestPickup || !picked.Add(order.Id))
                    return new(false, "invalid pickup " + order.Id);
                onboard.Add(order.Id);
            }
            if (edge.Kind == "delivery")
            {
                var order = orders[edge.Order!];
                if (edge.To.Site != order.Delivery || edge.To.Time > order.LatestDelivery || !onboard.Remove(order.Id))
                    return new(false, "invalid delivery " + order.Id);
            }
            int load = onboard.Sum(id => orders[id].Exclusive ? route.Capacity : orders[id].Quantity);
            if (load > route.Capacity) return new(false, "capacity exceeded");
            peak = Math.Max(peak, load);
            cost += edge.Cost;
            current = edge.To;
        }
        if (current != route.End || onboard.Count != 0 || picked.Count == 0) return new(false, "unfinished route/service");
        return new(true, "feasible", cost, peak);
    }
}
