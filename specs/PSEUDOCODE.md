# Pseudocode notation

The `.pseudo` files specify the separate vehicle/package route algorithm. Each module defines its purpose, inputs, outputs, references and procedures. `ASSUMPTIONS` records modeling or algorithm choices; `IMPLEMENTATION` notes describe the relationship to C# routines. Differences are summarized in [technical notes](../docs/TECHNICAL_NOTES.md).

Records and signatures follow [INTERFACES.md](INTERFACES.md). Maps have declared defaults, route records are ordered, and keys are immutable. `REQUIRE` checks a precondition. Solver results contain both termination status and solution availability. Absent objectives and bounds remain unknown.

Network construction, master, pricing and export use the same cost convention. Physical routes, artificial initialization objects and unserved-package penalty routes have distinct roles. An arc's identity includes the full endpoint states and times.
