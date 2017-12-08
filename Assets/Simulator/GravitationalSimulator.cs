using System.Collections.Generic;

using UnityEngine;

public class GravitationalSimulator : MonoBehaviour {
    [Header("Simulation Options")]

    public bool IntegrateMovement =
        true;
    public bool PreferBarnesHut =
        true;
    public bool EnableLog =
        true;
    public bool ShowQuadTree =
        true;
    public bool ShowInteractions =
        true;

    [Header("Simulation Parameters")]

    public float UniverseSize =
        140.0f;
    public float SimulationSofteningLength =
        100.0f;
    private float simulationSofteningLengthSquared;

    public float SimulationTime =
        float.PositiveInfinity;
    public float SimulationDeltaTime =
        -1.0f; /* Set any negative value to use Unity's fixed update DeltaTime */

    [Header("Simulation IO")]

    public bool ReplaySimulationFromFile =
        false;
    public string SimulationInputFilePath =
        "";
    public bool SaveSimulationToFile =
        false;
    public string SimulationOutputFilePath =
        "";

    private List<PlanetController> planets =
        new List<PlanetController>();

    private QuadTreeNode quadtree;

    private long interactions = 
        0;

    public void AddPlanet (PlanetController planet) {
        planets.Add (planet);
    }

    void Start () {
        simulationSofteningLengthSquared =
            SimulationSofteningLength * SimulationSofteningLength;
        
        WireframeController[] wireframeControllers =
            FindObjectsOfType<WireframeController> ();

        foreach (var wireframeController in wireframeControllers) {
            wireframeController.Planets =
                planets;
        }

        quadtree =
            new QuadTreeNode (UniverseSize);
    }

    void FixedUpdate () {
        interactions =
            0;

        if (PreferBarnesHut) {
            SimulateWithBarnesHut ();
        } else {
            SimulateWithBruteforce ();
        }

        if (EnableLog) {
            Debug.Log(
                "Barnes-Hut: " + PreferBarnesHut +
                "; Planets: "  + planets.Count +
                "; Interactions: " + interactions
            );
        }
    }

    void Update () {
        planets.RemoveAll (planet => !planet.IsAlive);
    }

    void OnDrawGizmos () {
        if (ShowQuadTree) {
            DrawDebugGizmos ();
        }
    }

    private void SimulateWithBarnesHut () {
        quadtree.Clear ();
        quadtree.TryAddRange (planets);

        if (!IntegrateMovement)
            return;

        foreach (PlanetController planet in planets) {
            planet.Acceleration =
                quadtree.CalculateForce (
                    planet, CalculateNewtonGravityAcceleration
                );
        }
    }

    private void SimulateWithBruteforce () {
        if (!IntegrateMovement)
            return;

        foreach (PlanetController planet in planets) {
            if (!planet.IsAlive)
                continue;

            Vector2 acceleration = Vector2.zero;
            foreach (PlanetController anotherPlanet in planets) {
                if (planet == anotherPlanet || !anotherPlanet.IsAlive)
                    continue;

                acceleration +=
                    CalculateNewtonGravityAcceleration (
                        planet, anotherPlanet
                    );
            }

            planet.Acceleration =
                acceleration;
        }
    }

    private Vector2 CalculateNewtonGravityAcceleration (
                        IBody firstBody,
                        IBody secondBody
                    ) {
        ++interactions;
        if (ShowInteractions)
            DrawDebugLines (firstBody, secondBody);

        Vector2 acceleration =
            Vector2.zero;

        Vector2 galacticPlaneR =
            secondBody.Position - firstBody.Position;

        float distanceSquared =
            galacticPlaneR.sqrMagnitude + simulationSofteningLengthSquared;
        float distanceSquaredCubed =
            distanceSquared * distanceSquared * distanceSquared;
        float inverse =
            1.0f / Mathf.Sqrt (distanceSquaredCubed);
        float scale =
            secondBody.Mass * inverse;

        acceleration +=
            galacticPlaneR * scale;

        return acceleration;
    }

    private void DrawDebugGizmos () {
        if (!Application.isPlaying)
            return;

        Gizmos.matrix =
            Matrix4x4.TRS (
                Vector3.zero,
                Quaternion.AngleAxis (
                    90.0f, Vector3.right
                ),
                Vector3.one
            );

        quadtree.Visit (node => {
            bool shouldContinue =
                true;

            Gizmos.DrawWireCube (
                node.Bounds.Center,
                node.Bounds.Size
            );

            return shouldContinue;
        });
    }

    private void DrawDebugLines (
                     IBody firstBody,
                     IBody secondBody
                 ) {
        Vector2 from =
            firstBody.Position;
        Vector2 to =
            secondBody.Position;
        Color debugLineColor =
            secondBody is QuadTreeNode ? Color.green : Color.red;

        Debug.DrawLine (
            new Vector3 (from.x, 0.0f, from.y),
            new Vector3 (to.x,   0.0f, to.y),
            debugLineColor
        );
    }
}

