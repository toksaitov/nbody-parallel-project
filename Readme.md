Accelerating an N-body Simulation
=================================

![N-body Simulation](https://i.imgur.com/P3eAqMW.png)

The primary goal of this project is to optimize a C program that performs direct [N-body simulation](http://www.scholarpedia.org/article/N-body_simulations_(gravitational)) on a sample dataset of a small planetary system. The optimization should be achieved by distributing computation not only across the cores of a single machine but also across multiple computers in a cluster connected via a high-speed local network. To facilitate writing distributed code, you must utilize a library that implements the [MPI standard](https://www.mcs.anl.gov/research/projects/mpi), specifically, the Open MPI [library](https://www.open-mpi.org).

The project comprises two programs. The first is a serial C program that you are tasked with converting into a distributed application using Open MPI. Its code is available [here](https://github.com/toksaitov/nbody-starter). This program generates a test dataset with several planets, each defined by random masses, positions in space, and initial velocities. It then performs a slow but accurate brute-force $O(N^2)$ direct N-Body simulation over a user-specified period. The outcomes, in terms of calculated accelerations at different time points, are displayed on the screen. This output can be redirected to a file for replay in the second program. The second program is a Unity-based application designed to visualize the planetary system and its evolution over time. Your involvement with the second application is not required; it serves merely to add gravity (pun intended) and context to the project.

You can view the entire process of utilizing all the programs in the video below. The video also showcases the distributed version developed by your instructor, who opts for the MPICH library over Open MPI. Consequently, the instructor uses `mpiexec` instead of `mpirun`, with a few different flags (`-n` instead of `-np`, and `-f` instead of `-hostfile`). Additionally, the instructor simulates 100 bodies instead of 128. Despite these differences, the workflow remains largely the same.

[![Results](https://i.imgur.com/AWRaQH4.png)](https://drive.google.com/open?id=1LLFR2NcRhT2R43SCoZ69322wU0EMG35S)

## Tasks

1. Upload or clone the `nbody.c` and `nbody-mpi.c` source files to our course server `auca.space` from [this repository](https://github.com/toksaitov/nbody-starter). You must measure performance on `auca.space` connected to `peer.auca.space`, not on your computer.
2. Ensure that SSH clients on your computer and both the `auca.space` and `peer.auca.space` servers are correctly configured to allow password-less connections between your accounts on these server machines. Consult the lab videos for more information.
3. Compile `nbody.c` and its parallel version `nbody-mpi.c` by running `make`.
4. Execute the serial program with the command `time ./nbody 100 0.01 128 10000 100 100 > SerialSimulation.txt`.
5. Use `scp` to copy the generated `SerialSimulation.txt` file to your computer.
6. Clone the visualization Unity project from the repository mentioned in this Readme file. Then, open it in Unity Editor 2022.
7. Open the `Main.unity` scene file from the Project panel and the `Assets` directory.
8. Select the `Simulator` object in the Hierarchy panel.
9. In the Inspector panel, under the `Gravitational Simulator (Script)`, ensure that the `Replay Simulation from File` option is checked.
10. In the `Simulation File` field, select the file you downloaded in Step 5. You may need to move the file into the `SimulationData` directory of the Unity project first to be able to select it.
11. Start the replay by clicking on the Play button in Unity. Enjoy watching the planetary bodies form stable orbits around each other after a big explosive start (not a bang in our case).
12. Return to the `auca.space` server and start modifying the `nbody-mpi.c` file. Use the Open MPI library and its Collective Communication functions to distribute N-body computations.
13. Compile your `nbody-mpi.c` code with `make`.
14. First, test your code on one computer with `time mpirun -np 16 ./nbody-mpi 100 0.01 128 10000 100 100 > ParallelSimulation.txt`.
15. Compare the output files with `vimdiff *.txt`. The simulation files should be identical.
16. Create a file named `hostfile` with the following content

    ```
    auca slots=16
    auca-peer slots=16
    ```

17. Test your code on two computers using the command `time mpirun -hostfile hostfile -np 32 ./nbody-mpi 100 0.01 128 10000 100 100 > ParallelSimulation.txt`. Ensure that the simulation output files remain identical.
18. Copy `ParallelSimulation.txt` into the `SimulationData` folder of the Unity project. Rerun the visualization to verify that it functions as expected.
19. Experiment with varying the number of processors and bodies to determine when it is most effective to use your MPI solution on one or two machines.

## Rules

* Do NOT profile code anywhere except on our server at `auca.space` connected to `peer.auca.space`.
* Avoid procrastination and completing work at the last moment. If the servers become overloaded close to the deadline, obtaining accurate measurements will be difficult. Extensions will not be granted for this reason.
* Do NOT alter the output format, as it will render the Unity visualization non-functional.
* Utilize the provided `Makefile` exclusively for compiling the code.
* Do NOT modify the core N-body simulation algorithm. It MUST remain the brute-force [direct N-body algorithm](http://www.scholarpedia.org/article/N-body_simulations_(gravitational)#Direct_methods), known for its high accuracy. Despite its quadratic complexity, which renders it impractical for large celestial systems on a single core, it is essential for maintaining the integrity of the simulation. For those interested in more scalable, approximated solutions, the Unity project includes an implementation of the Barnes-Hut Tree Code method, which operates at $O(N\log(N))$ complexity. This can be accessed by disabling replays in the `Gravitational Simulator (Script)` within the Inspector panel. You are encouraged to compare its performance with the direct method on your computer by experimenting with other checkboxes. Ensure all settings are reset to their original state before testing your simulation files.
* Refrain from using assembly tricks, intrinsics, or multithreading APIs to enhance your code's performance. The use of Open MPI functions is permitted solely for accelerating the `nbody-mpi.c` code.

## Recommendations

* Try understanding the serial algorithms first in `nbody.c` to identify the most suitable parts of the code for parallel and distributed computation.
* A simple solution with point-to-point MPI functions such as `MPI_Send` and `MPI_Recv` may be a good start. However, to achieve the best performance, consider using some collective communication functions from the following list (not all of them may be necessary):

    * `MPI_Bcast`
    * `MPI_Scatter`, `MPI_Gather`, `MPI_Allgather`,
    * `MPI_Reduce`, `MPI_Allreduce`

* You can use the following code to define a custom MPI date type for the `body_t` struct:

```c
static MPI_Datatype create_body_t_mpi_type()
{
    static const int count = 7;
    const int block_lengths[] = {
        1, 1,
        1, 1,
        1, 1,
        1
    };
    MPI_Aint offsets[] = {
        offsetof(body_t, x),  offsetof(body_t, y),
        offsetof(body_t, ax), offsetof(body_t, ay),
        offsetof(body_t, vx), offsetof(body_t, vy),
        offsetof(body_t, mass)
    };
    const MPI_Datatype types[] = {
        MPI_FLOAT, MPI_FLOAT,
        MPI_FLOAT, MPI_FLOAT,
        MPI_FLOAT, MPI_FLOAT,
        MPI_FLOAT
    };

    MPI_Datatype type;
    MPI_Type_create_struct(count, block_lengths, offsets, types, &type);
    MPI_Type_commit(&type);

    return type;
}
```

* Use the function in the following way to create your type:

```c
MPI_Datatype mpi_body_t = create_body_t_mpi_type();
```

* Don't forget to deallocate the created type before `MPI_Finalize()`:

```c
MPI_Type_free(&mpi_body_t);
```

* To reuse an array for sending and receiving data with `MPI_Allgather`, you must use the `MPI_IN_PLACE` constant as a `sendbuf`, as outlined [here](https://www.open-mpi.org/doc/v3.0/man3/MPI_Allgather.3.php). This approach is particularly useful when you want to optimize memory usage by avoiding additional buffer allocation.

## Deadline

Check Moodle for information about the deadlines.

## Documentation

man gcc
man mpicc
man mpirun

## Links

### C, GDB

* [Beej's Guide to C Programming](https://beej.us/guide/bgc)
* [GDB Quick Reference](http://users.ece.utexas.edu/~adnan/gdb-refcard.pdf)

### MPI

* [MPI Tutorial](https://mpitutorial.com)
* [MPI LLNL HPC Tutorials](https://hpc-tutorials.llnl.gov/mpi)
* [Open MPI Official Documentation](https://www.open-mpi.org/doc)

## Books

* C Programming: A Modern Approach, 2nd Edition by K. N. King
