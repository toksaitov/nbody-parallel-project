N-body Simulation
=================

In this task, you need to write a command-line MPI application that can perform
the [N-body](https://en.wikipedia.org/wiki/N-body_simulation) simulation in
parallel on a distributed-memory system.

For this work, you will get a sample Unity project that you can use as a
reference to understand the algorithms and the simulation itself. The solution
in Unity is single-threaded and thus slow for high numbers of planets.

Your program should generate a similar sample set of planetary bodies as it is
generated in the GenerateDebugData method in the PlayerController.cs file.

The PlanetController class should be turned into a simple array or struct to
store position, mass, velocity, and other required state variables for each
planet.

The program should only calculate the acceleration and output it to a file for a
certain simulation time period (allow to specify it as command-line parameters).

The output should start with two numbers. On the first line output the number of
bodies in the system. On the next line, output the Delta Time of the simulation.
After that, write all the acceleration vector components for x and y separated
by a whitespace. Each acceleration vector should be specified on a separate
line.

## Option #1

Write a program that performs the brute-force O(N^2) simulation outlined in the
method SimulateWithBruteforce in GravitationalSimulator.cs. You can get the
maximum grade just by doing this option.

## Option #2

For extra points, try to parallelize the Barnes-Hut approach outlined in the
method SimulateWithBarnesHut in GravitationalSimulator.cs. You can only do this
option to get the full grade and extra points.

