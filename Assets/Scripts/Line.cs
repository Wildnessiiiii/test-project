using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line
{
    const float verGradient = 1e5f;

    private float gradient;//梯度
    private float y_intercept;//截距
    private Vector2 pointOnline1;
    private Vector2 pointOnline2;
    private float gradientPerpendicular;//垂线斜率

    private bool approachSide;

    public Line(Vector2 pointOnline, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnline.x - pointPerpendicularToLine.x;
        float dy = pointOnline.y - pointPerpendicularToLine.y;

        if (dx == 0)
        {
            gradientPerpendicular = verGradient;
        }
        else
        {
            gradientPerpendicular = dy / dx;
        }

        if (gradientPerpendicular == 0)
        {
            gradient = verGradient;
        }
        else
        {
            gradient = -1 / gradientPerpendicular;
        }

        y_intercept = pointOnline.y - gradient * pointOnline.x;

        pointOnline1 = pointOnline;
        pointOnline2 = pointOnline1 + new Vector2(1, gradient);

        approachSide = false;
        approachSide = HasCrossedLine(pointPerpendicularToLine);

    }

    private bool EnsureSide(Vector2 point)
    {
        return (point.x - pointOnline1.x) * (pointOnline2.y - pointOnline1.y) > (point.y - pointOnline1.y) * (pointOnline2.x - pointOnline1.x);
    }


    public bool HasCrossedLine(Vector2 p)
    {
        return EnsureSide(p) != approachSide;
    }

    public void DrawWithGizmos(float length)
    {
        Vector3 lineDir = new Vector3(1, 0, gradient).normalized;
        Vector3 lineCentre = new Vector3(pointOnline1.x, 0, pointOnline1.y) + Vector3.up;
        Gizmos.DrawLine(lineCentre - lineDir * length / 2f, lineCentre + lineDir * length / 2f);
    }
}
