using System.Collections.Generic;

using UnityEngine;

class QuadTreeNode : IBody {
    public delegate bool Visitor (QuadTreeNode node);
    public delegate Vector2 ForceCalculator (IBody firstBody, IBody secondBody);

    private const int DefaultCapacity =
        1;
    private float Theta =
        2.0f;

    public int Capacity { get; set; }
    public IBoundingVolume Bounds { get; set; }

    public float Size {
        get { return Bounds.Size.magnitude; }
    }

    public Vector2 Position {
        get { return centerOfMass / Mass; }
    }
    public  float Mass { get; set; }

    public QuadTreeNode [] Nodes {
        get; private set;
    }

    public List<PlanetController> Planets {
        get; private set;
    }
    public bool HasPlanets {
        get { return Planets.Count > 0; }
    }
    public PlanetController Planet {
        get { return HasPlanets ? Planets [0] : null; }
    }

    public bool IsLeaf {
        get { return Nodes [0] == null; }
    }

    private Vector2 centerOfMass;

    public QuadTreeNode (float sideLength)
        : this(new BoundingRectangle(sideLength)) { }

    public QuadTreeNode (IBoundingVolume bounds) {
        Capacity =
            DefaultCapacity;
        Bounds =
            bounds;

        centerOfMass =
            bounds.Center;
        Mass =
            0.0f;

        Nodes =
            new QuadTreeNode [4];
        Planets =
            new List<PlanetController> (Capacity);
    }

    public bool TryAdd (PlanetController planet) {
        bool result =
            false;

        if (!planet.IsAlive) {
            return result;
        }

        if (Bounds.Contains (planet.Position)) {
            Mass +=
                planet.Mass;
            centerOfMass +=
                planet.Position * planet.Mass;

            if (!IsLeaf) {
                AddToFirstFittingNode (planet);
            } else {
                if (Planets.Count < Capacity) {
                    Planets.Add (planet);
                } else {
                    Subdivide ();

                    SinkPlanetsToNodes ();
                    AddToFirstFittingNode (planet);
                }
            }

            result =
                true;
        }

        return result;
    }

    public void TryAddRange (IEnumerable<PlanetController> planets) {
        foreach (var planet in planets)
            TryAdd (planet);
    }

    public void Visit (Visitor visitor) {
        if (visitor (this))
            foreach (var node in Nodes)
                if (node != null) node.Visit (visitor);
    }

    public Vector2 CalculateForce(
                       PlanetController planet,
                       ForceCalculator forceCalculator
                   ) {
        Vector2 acceleration =
            Vector2.zero;

        Visit (node => {
            bool shouldContinue = false;
            if (node.IsLeaf) {
                if (node.HasPlanets) {
                    acceleration +=
                        forceCalculator (planet, node.Planet);
                }
            } else {
                float size =
                    node.Size;
                float distance =
                    Vector2.Distance (
                        planet.Position,
                        node.Position
                    );

                shouldContinue = size / distance >= Theta;
                if (!shouldContinue) {
                    acceleration +=
                        forceCalculator (planet, node);
                }
            }

            return shouldContinue;
        });

        return acceleration;
    }

    public void Clear () {
        for (int i = 0; i < Nodes.Length; ++i)
            Nodes [i] =
                null;

        Planets.Clear ();
    }

    private void Subdivide () {
        var quadrants =
            Bounds.Subdivide ();

        for (int i = 0; i < Nodes.Length && i < quadrants.Length; ++i) {
            Nodes [i] = new QuadTreeNode (quadrants [i]) {
                Capacity =
                    Capacity
            };
        }
    }

    private void AddToFirstFittingNode (PlanetController planet) {
        foreach (var node in Nodes)
            if (node.TryAdd (planet)) break;
    }

    private void SinkPlanetsToNodes () {
        foreach (var planet in Planets)
            AddToFirstFittingNode (planet);

        Planets.Clear ();
    }
}

