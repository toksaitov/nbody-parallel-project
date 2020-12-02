N-body Simulation
=================

[![Results](https://i.imgur.com/AWRaQH4.png)](https://drive.google.com/open?id=1LLFR2NcRhT2R43SCoZ69322wU0EMG35S)

In this task, you need to write a command-line MPI application that can perform
the [N-body](http://www.scholarpedia.org/article/N-body_simulations_(gravitational)) simulation in
parallel on a distributed-memory system.

For this work, you will get a sample Unity project that you can use as a
reference to understand the algorithms and the simulation itself. The solution
in Unity is single-threaded and thus slow for high numbers of planets.

Your program should generate a similar sample set of planetary bodies as it is
generated in the GenerateDebugData method in the PlayerController.cs file. Some
constant values can be found by opening the Unity editor and inspecting the
planet's and the simulator's prefabs.

The PlanetController class should be turned into a simple array or struct to
store position, mass, velocity, and other required state variables for each
planet.

The program should only calculate the acceleration and output it to a file for a
certain simulation time period (allow to specify it as command-line parameters).

The output should start with three numbers. On the first line output the number
of bodies in the system. On the second line output the time period for the
simulation.  On the next last line, output the Delta Time of the simulation.
Next, output the initial configuration for each body printing the position,
acceleration, and velocity vectors on a separate line with a whitespace
character between vector components. Finish printing the initial configuration
with a mass of the body placed on a separate line.  After that, write all the
acceleration vector components for _x_ and _y_ separated by a whitespace. Each
acceleration vector should be specified on a separate line.

Here you can find a sample output for two bodies and the simulation with a time
period of 1.0 and delta time of 0.1.

```
2
1.000000
0.100000
0.394383 0.783099
0.000000 0.000000
89.849129 15.431899
12984.399414
0.335223 0.768230
0.000000 0.000000
-54.764771 8.345478
7777.747070
-0.460107 -0.115646
0.768116 0.193062
-20.573603 -1.021606
34.346176 1.705500
-8.062413 -0.396962
13.459630 0.662700
-4.028891 -0.197784
6.725948 0.330186
-2.384562 -0.116885
3.980859 0.195132
-1.569821 -0.076878
2.620705 0.128343
-1.109973 -0.054325
1.853022 0.090691
-0.825804 -0.040399
1.378621 0.067443
-0.638174 -0.031209
1.065386 0.052102
-0.507888 -0.024831
0.847883 0.041454
```

You can also find the starter code with a serial implementation in C that you
MUST use [here](https://github.com/toksaitov/nbody-starter);

## Tasks

Write a program that performs the brute-force O(N^2) simulation outlined in the
method `SimulateWithBruteforce` in `GravitationalSimulator.cs` in a distributed
way by using MPI. Use the [starter](https://github.com/toksaitov/nbody-starter)
code and write your solution in the `nbody-mpi.c` file. Test your solution on
the `sys1` and `sys2` course machines. Ensure that your distributed system runs
all the servers and on all the cores of their CPUs. Consult the course video
recordings about the information on how to run your solution on all machines at
the same time.

## What to Submit

1. In your private course repository that was given to you by the instructor
   during the lecture, create the path `project-2/`.
2. Put the `nbody-mpi.c` file into that directory.
3. Commit and push your repository through Git. Submit the last commit ID to
   Canvas before the deadline.

## Deadline

Check Canvas for information about the deadline for the first part.
