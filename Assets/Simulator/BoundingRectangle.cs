using UnityEngine;

struct BoundingRectangle : IBoundingVolume {
    public Vector2 Minimum;
    public Vector2 Maximum;

    public Vector2 Size {
        get { return Maximum - Minimum;  }
        set { Maximum = Minimum + value; }
    }

    public float X {
        get { return Minimum.x;  }
        set { Minimum.x = value; }
    }

    public float Y {
        get { return Minimum.y;  }
        set { Minimum.y = value; }
    }

    public float CenterX {
        get { return Minimum.x + Width * 0.5f;  }
    }

    public float CenterY {
        get { return Minimum.y + Height * 0.5f;  }
    }

    public Vector2 Center {
        get { return new Vector2 (CenterX, CenterY);  }
    }

    public float Width {
        get { return Size.x; }
        set { Maximum.x = Minimum.x + value; }
    }

    public float Height {
        get {  return Size.y; }
        set { Maximum.y = Minimum.y + value; }
    }

    public float Left {
        get { return Minimum.x;  }
        set { Minimum.x = value; }
    }

    public float Right {
        get { return Maximum.x;  }
        set { Maximum.x = value; }
    }

    public float Top {
        get { return Minimum.y;  }
        set { Minimum.y = value; }
    }

    public float Bottom {
        get { return Maximum.y;  }
        set { Maximum.y = value; }
    }

    public BoundingRectangle (float sidelength)
        : this (new Vector2 (-sidelength, -sidelength),
                new Vector2 ( sidelength,  sidelength)) { }

    public BoundingRectangle (BoundingRectangle anotherBoundingRectangle)
        : this (anotherBoundingRectangle.Minimum,
                anotherBoundingRectangle.Maximum) { }

    public BoundingRectangle (Vector2 minimum, Vector2 maximum) {
        Minimum =
            minimum;
        Maximum =
            maximum;
    }

    public bool IntersectsWith (IBoundingVolume anotherBoundingVolume) {
        if (anotherBoundingVolume != null)
            if (anotherBoundingVolume is BoundingRectangle)
                return IntersectsWith ((BoundingRectangle) anotherBoundingVolume);

        return false;
    }

    public bool IntersectsWith (BoundingRectangle anotherBoundingRectangle) {
        if (anotherBoundingRectangle.Minimum.x > Maximum.x)
            return false;
        if (anotherBoundingRectangle.Minimum.y > Maximum.y)
            return false;
        if (Minimum.x > anotherBoundingRectangle.Maximum.x)
            return false;
        if (Minimum.y > anotherBoundingRectangle.Maximum.x)
            return false;

        return true;
    }

    public bool Contains (IBoundingVolume anotherBoundingVolume) {
        if (anotherBoundingVolume != null)
            if (anotherBoundingVolume is BoundingRectangle)
                return Contains ((BoundingRectangle) anotherBoundingVolume);

        return false;
    }

    public bool Contains (float x, float y) {
        return Minimum.x <= x &&
               Minimum.y <= y &&
               x <= Maximum.x &&
               y <= Maximum.y;
    }

    public bool Contains (Vector2 position) {
        return Minimum.x <= position.x &&
               Minimum.y <= position.y &&
               position.x <= Maximum.x &&
               position.y <= Maximum.y;
    }

    public bool Contains (BoundingRectangle anotherBoundingRectangle) {
        return Minimum.x <= anotherBoundingRectangle.Minimum.x &&
               Minimum.y <= anotherBoundingRectangle.Minimum.y &&
               anotherBoundingRectangle.Maximum.x <= Maximum.x &&
               anotherBoundingRectangle.Maximum.y <= Maximum.y;
    }

    public IBoundingVolume [] Subdivide () {
        var result =
            new IBoundingVolume [4];

        float halfWidth =
            Width * 0.5f;
        float halfHeight =
            Height * 0.5f;

        int i = 0;
        for (int y = 0; y < 2; ++y) {
            for (int x = 0; x < 2; ++x) {
                var quadrant =
                    new BoundingRectangle (this);

                quadrant.Minimum +=
                    new Vector2 (
                        x * halfWidth,
                        y * halfHeight
                    );

                quadrant.Width =
                    halfWidth;
                quadrant.Height =
                    halfHeight;

                result [i++] =
                    new BoundingRectangle (quadrant);
            }
        }

        return result;
    }
}

