# Reading guide

The [system diagram](README.md#the-ltadr-system), [decomposition method](README.md#routing-method) and [paper results](README.md#results-in-the-paper) provide an overview. For the algorithm, read **model → controller → master and pricing → dynamic updates**.

| Purpose | Entry |
| --- | --- |
| Understand the mathematical method | [Model](docs/MODEL.md), [40-equation map](docs/PAPER_MAP.md) |
| Follow the complete algorithm | [Module index](pseudocode/README.md), [controller](pseudocode/Program.pseudo) |
| Build the network and initial pool | [Network rules](specs/NETWORK_RULES.md), [initialization](pseudocode/ArcModel.pseudo), [coefficients](pseudocode/A_B_generator.pseudo) |
| Follow master, pricing and branching | [Master](pseudocode/GeneratingMasterProblem.pseudo), [pricing dispatch](pseudocode/GeneratingSubProblem.pseudo), [branching](pseudocode/Branching.pseudo) |
| Follow periodic updates | [Dynamic update](pseudocode/DynamicUpdate.pseudo), [order screening](pseudocode/OrderManager.pseudo), [commitments](pseudocode/DynConstrGenerater.pseudo) |
| Run a small case | [Two examples](examples/README.md), [build instructions](docs/BUILD.md) |
| Compare code and pseudocode | [Implementation map](docs/COVERAGE.md), [source map](docs/SOURCE_MAP.md), [technical notes](docs/TECHNICAL_NOTES.md) |

The [example test results](validation/RESULTS.md) document the synthetic cases. The [article](https://doi.org/10.1177/03611981261468762) provides the experimental settings and full numerical results.
