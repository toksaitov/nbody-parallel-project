using UnityEngine;

interface IBoundingVolume {
    Vector2 Center { get; }
    Vector2 Size   { get; }

    bool IntersectsWith (IBoundingVolume anotherBoundingVolume);

    bool Contains (IBoundingVolume anotherBoundingVolume);
    bool Contains (float x, float y);
    bool Contains (Vector2 point);

    IBoundingVolume[] Subdivide ();
}

