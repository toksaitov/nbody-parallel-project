Accelerating an N-body Simulation
=================================

![N-body Simulation](https://i.imgur.com/P3eAqMW.png)

The main goal of this project is to optimize a C program that does direct [N-body simulation](http://www.scholarpedia.org/article/N-body_simulations_(gravitational)) on a sample data set of a small planetary system. You have to do it by distributing computation between not only the cores of one machine but between the cores of multiple computers in a cluster connected through a high-speed local network. To help you write distributed code, you must use a library implementing the [MPI standard](https://www.mcs.anl.gov/research/projects/mpi), specifically the [MPICH library](https://www.mpich.org).

The project includes two programs. The first program is the serial C program you must make distributed using MPICH. You can find its code [here](https://github.com/toksaitov/nbody-starter). The program generates a test data set with several planets of random masses, positions in space, and initial velocities. Then it performs a slow but precise brute-force $O(N^2)$ direct N-Body simulation for a period specified by the user. Finally, it prints the calculated accelerations at different time points to the screen that can be redirected to a file and replayed from it in the second program. The second program is a Unity-based application that can visualize the planetary system to see its evolution through time. You don't have to do anything with the second application. It is just a cool 3-D program to give some gravity (pun not intended) and meaning to the whole exercise.

You can see the whole process of using all the programs in the video below. The video also includes the demonstration of the distributed version written by your instructor.

[![Results](https://i.imgur.com/AWRaQH4.png)](https://drive.google.com/open?id=1LLFR2NcRhT2R43SCoZ69322wU0EMG35S)

## Tasks

1. Upload/clone the `nbody.c` and `nbody-mpi.c` source files to our course server `auca.space` from [this repository](https://github.com/toksaitov/nbody-starter). You have to measure performance on `auca.space` connected to `peer.auca.space` and not on your computer.
2. Ensure that SSH clients on your computer and both `auca.space` with `peer.auca.space` servers are correctly configured to allow your accounts password-less connections between the server machines. Consult lab videos for more info.
3. Compile `nbody.c` and its copy in `nbody-mpi.c` by running `make`.
4. Run the serial program with `time ./nbody 100 0.01 100 10000 100 100 > SerialSimulation.txt`.
5. Copy the generated `SerialSimulation.txt` with `scp` to your computer.
6. Clone the visualization Unity project from the repository with this Readme file. Then, open it in the Unity 2021 editor.
7. Open the `Main.unity` scene file from the Project panel and the `Assets` directory.
8. Select the `Simulator` object in the Hierarchy panel.
9. In the Inspector panel under the `Gravitational Simulator (Script)`, ensure the `Replay Simulation from File` is checked.
10. In the `Simulation File`, select the file downloaded in Step 5. You may have to put the file into the `SimulationData` directory of the Unity project first to be able to select it.
11. Start the replay by clicking on the Play button in Unity. Enjoy watching the planetary bodies forming stable orbits around each other eventually after the big explosive start (not bang in our case).
12. Go back to the `auca.space` server and start working on the `nbody-mpi.c` file. Use MPICH library and its Collective Communication functions to make N-body computations distributed.
13. Compile your `nbody-mpi.c` code with `make`.
14. Test your code on one computer first with `time mpiexec -n 64 ./nbody-mpi 100 0.01 100 10000 100 100 > ParallelSimulation.txt`.
15. Compare the output files with `vimdiff *.txt`. The simulation files must be the same.
16. Create the `machinefile` with the following content

    ```
    auca:64
    auca-peer:32
    ```

17. Test your code on two computers with `time mpiexec -f machinefile -n 64 ./nbody-mpi 100 0.01 100 10000 100 100 > ParallelSimulation.txt`. Ensure the simulation files are still the same.
18. Ensure that `time` measurements improve from `nbody` to `nbody-mpi` on one and finally two computers on average after several runs.
19. Copy the `ParallelSimulation.txt` into the `SimulationData` of the Unity project. Rerun the visualization. Ensure it still works the same way.

## Rules

* Do NOT profile code anywhere but on our server at `auca.space` connected to `peer.auca.space`.
* Do NOT procrastinate and leave the work to the very last moment. If the servers are overloaded close to the deadline, you will not be able to get good measurements. We will not give any extensions for that reason.
* Do NOT change the output format, or the Unity visualization will not work.
* Use the provided `Makefile` to compile the code and nothing else.
* Do NOT change the core N-body simulation algorithm. It MUST stay the brute-force [direct N-Body algorithm](http://www.scholarpedia.org/article/N-body_simulations_(gravitational)#Direct_methods). It is known for its high accuracy, but its quadratic complexity makes it unusable for any celestial system with more than ~20000 bodies. You can check the Unity project's code if you are interested in approximated but more scalable $O(N\log(N))$ solutions. It contains an implementation of the Barnes & Hut Tree Code method. You can check it by disabling replays in the `Gravitational Simulator (Script)` under the Inspector panel. You can even compare its speed on your computer to the direct method by experimenting with other checkboxes. Remember to return everything to its original state before testing your simulation files.
* Do NOT use assembly tricks, intrinsics, or multithreading APIs to boost the speed of your code. You can only use MPICH functions to speed up the `nbody-mpi.c` code.

## Recommendations

1. Try understanding the serial algorithms first in `nbody.c` to figure out the best part of the code to compute in a parallel and distributed way.
2. A simple solution with point-to-point MPI functions such as `MPI_Send` and `MPI_Recv` may be a good start, but to get the best performance you will have to consider using some (not all) collective communication functions from the following list:

    * `MPI_Bcast`
    * `MPI_Scatter`, `MPI_Gather`, `MPI_Allgather`,
    * `MPI_Reduce`, `MPI_Allreduce`

## What to Submit

1. In your private course repository that was given to you by the instructor during the lecture, create the path `project-4/`.
2. Put the `nbody-mpi.c` file into that directory.
3. Commit and push your repository through Git. Submit the last commit URL to Canvas before the deadline.

## Deadline

Check Canvas for information about the deadlines.

## Documentation

man gcc
man mpicc
man mpiexec

## Links

### C, GDB

* [Beej's Guide to C Programming](https://beej.us/guide/bgc)
* [GDB Quick Reference](http://users.ece.utexas.edu/~adnan/gdb-refcard.pdf)

### MPI

* [MPI Tutorial](https://mpitutorial.com)
* [MPI LLNL HPC Tutorials](https://hpc-tutorials.llnl.gov/mpi)
* [MPICH Official Documentation](https://www.mpich.org/documentation/guides)

## Books

* C Programming: A Modern Approach, 2nd Edition by K. N. King
