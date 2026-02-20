using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Google.OrTools.Api.Services
{
    public class Geo
    {
        public static bool IsPointInCircle(PointF[] bounds, PointF point)
        {
            var center_lng = (bounds[1].X + bounds[0].X) / 2;
            var center_lat = (bounds[1].Y + bounds[0].Y) / 2;
            var radius = distanceInKm(center_lat, center_lng, center_lat, bounds[0].X);

            var dist = distanceInKm(center_lat, center_lng, point.Y, point.X);
            if (dist <= radius)
                return true;
            else
                return false;
        }

        public static bool IsPointInCircle(PointF circle, PointF point, double radius)
        {
            var dist = distanceInKm(circle.Y, circle.X, point.Y, point.X);
            if (dist <= radius)
                return true;
            else
                return false;
        }

        public static bool IsPointInPolygon(PointF[] polygon, PointF point)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < point.Y && polygon[j].Y >= point.Y || polygon[j].Y < point.Y && polygon[i].Y >= point.Y)
                {
                    if (polygon[i].X + (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < point.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public static bool IsPointInRectangle(PointF[] bounds, PointF point)
        {
            if (bounds?.Length != 2)
                return false;

            float lat_min = (bounds[0].Y < bounds[1].Y ? bounds[0].Y : bounds[1].Y);
            float lat_max = (bounds[0].Y < bounds[1].Y ? bounds[1].Y : bounds[0].Y);
            float lng_min = (bounds[0].X < bounds[1].X ? bounds[0].X : bounds[1].X);
            float lng_max = (bounds[0].X < bounds[1].X ? bounds[1].X : bounds[0].X);

            if (lat_min <= point.Y && lat_max >= point.Y && lng_min <= point.X && lng_max >= point.X)
                return true;
            else
                return false;
        }

        public static double distanceInKm(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6378.1370; // Radius of the earth in km
            var dLat = deg2rad(lat2 - lat1);  // deg2rad below
            var dLon = deg2rad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in km
            return d;
        }

        public static double DistanciaRaio(double en_lat_orig, double en_long_orig, double en_lat_dest, double en_long_dest)
        {
            if (en_lat_orig == en_lat_dest && en_long_orig == en_long_dest)
                return 0;
            double vn_theta = en_long_orig - en_long_dest;
            double vn_dist = Math.Sin(deg2rad(en_lat_orig)) * Math.Sin(deg2rad(en_lat_dest)) + Math.Cos(deg2rad(en_lat_orig)) * Math.Cos(deg2rad(en_lat_dest)) * Math.Cos(deg2rad(vn_theta));
            vn_dist = Math.Acos(vn_dist);
            vn_dist = rad2deg(vn_dist);
            vn_dist = vn_dist * 60 * 1.1515;
            //if (ee_unit == MeasureUnit.Kilometers)
            vn_dist = vn_dist * 1.609344 * 1000;
            //else if (ee_unit == MeasureUnit.NauticalMile)
            //    vn_dist = vn_dist * 0.8684;
            return (vn_dist);
        }
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        public static double deg2rad(double en_deg)
        {
            return (en_deg * Math.PI / 180.0);
        }
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        public static double rad2deg(double en_rad)
        {
            return (en_rad / Math.PI * 180.0);
        }

        public static bool polyCheck(PointF[] polygon, PointF point)
        {
            int j = polygon.Length - 1;
            bool c = false;
            for (int i = 0; i < polygon.Length; j = i++)
                c ^= polygon[i].Y > point.Y ^ polygon[j].Y > point.Y && point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X;
            return c;
        }

        //http://csharphelper.com/blog/2016/01/find-a-polygon-union-in-c/
        // Return the union of the two polygons.
        public static List<PointF> FindPolygonUnion(List<PointF>[] polygons)
        {
            // Find the lower-leftmost point in either polygon.
            int cur_pgon = 0;
            int cur_index = 0;
            PointF cur_point = polygons[cur_pgon][cur_index];
            for (int pgon = 0; pgon < 2; pgon++)
            {
                for (int index = 0; index < polygons[pgon].Count; index++)
                {
                    PointF test_point = polygons[pgon][index];
                    if ((test_point.X < cur_point.X) ||
                        ((test_point.X == cur_point.X) &&
                         (test_point.Y > cur_point.Y)))
                    {
                        cur_pgon = pgon;
                        cur_index = index;
                        cur_point = polygons[cur_pgon][cur_index];
                    }
                }
            }

            // Create the result polygon.
            List<PointF> union = new List<PointF>();

            // Start here.
            PointF start_point = cur_point;
            union.Add(start_point);

            // Start traversing the polygons.
            // Repeat until we return to the starting point.
            for (; ; )
            {
                // Find the next point.
                int next_index = (cur_index + 1) % polygons[cur_pgon].Count;
                PointF next_point = polygons[cur_pgon][next_index];

                // Each time through the loop:
                //      cur_pgon is the index of the polygon we're following
                //      cur_point is the last point added to the union
                //      next_point is the next point in the current polygon
                //      next_index is the index of next_point

                // See if this segment intersects
                // any of the other polygon's segments.
                int other_pgon = (cur_pgon + 1) % 2;

                // Keep track of the closest intersection.
                PointF best_intersection = new PointF(0, 0);
                int best_index1 = -1;
                float best_t = 2f;

                for (int index1 = 0; index1 < polygons[other_pgon].Count; index1++)
                {
                    // Get the index of the next point in the polygon.
                    int index2 = (index1 + 1) % polygons[other_pgon].Count;

                    // See if the segment between points index1
                    // and index2 intersect the current segment.
                    PointF point1 = polygons[other_pgon][index1];
                    PointF point2 = polygons[other_pgon][index2];
                    bool lines_intersect, segments_intersect;
                    PointF intersection, close_p1, close_p2;
                    float t1, t2;
                    FindIntersection(cur_point, next_point, point1, point2,
                        out lines_intersect, out segments_intersect,
                        out intersection, out close_p1, out close_p2, out t1, out t2);

                    if ((segments_intersect) && // The segments intersect
                        (t1 > 0.001) &&         // Not at the previous intersection
                        (t1 < best_t))          // Better than the last intersection found
                    {
                        // See if this is an improvement.
                        if (t1 < best_t)
                        {
                            // Save this intersection.
                            best_t = t1;
                            best_index1 = index1;
                            best_intersection = intersection;
                        }
                    }
                }

                // See if we found any intersections.
                if (best_t < 2f)
                {
                    // We found an intersection. Use it.
                    union.Add(best_intersection);

                    // Prepare to search for the next point from here.
                    // Start following the other polygon.
                    cur_pgon = (cur_pgon + 1) % 2;
                    cur_point = best_intersection;
                    cur_index = best_index1;
                }
                else
                {
                    // We didn't find an intersection.
                    // Move to the next point in this polygon.
                    cur_point = next_point;
                    cur_index = next_index;

                    // If we've returned to the starting point, we're done.
                    if (cur_point == start_point) break;

                    // Add the current point to the union.
                    union.Add(cur_point);
                }
            }

            return union;
        }

        // Find the point of intersection between
        // the lines p1 --> p2 and p3 --> p4.
        private static void FindIntersection(PointF p1, PointF p2, PointF p3, PointF p4,
            out bool lines_intersect, out bool segments_intersect,
            out PointF intersection, out PointF close_p1, out PointF close_p2,
            out float t1, out float t2)
        {
            // Get the segments' parameters.
            float dx12 = p2.X - p1.X;
            float dy12 = p2.Y - p1.Y;
            float dx34 = p4.X - p3.X;
            float dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);
            t1 = ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34) / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new PointF(float.NaN, float.NaN);
                close_p1 = new PointF(float.NaN, float.NaN);
                close_p2 = new PointF(float.NaN, float.NaN);
                t2 = float.PositiveInfinity;
                return;
            }
            lines_intersect = true;

            t2 = ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12) / -denominator;

            // Find the point of intersection.
            intersection = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect = ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0) t1 = 0;
            else if (t1 > 1) t1 = 1;

            if (t2 < 0) t2 = 0;
            else if (t2 > 1) t2 = 1;

            close_p1 = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new PointF(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }

        //https://gis.stackexchange.com/questions/11203/how-to-determine-a-point-lays-within-a-polyline-in-arc-engine
        public static bool PointLaysOnLine(PointF[] points, PointF point, double tolerance)
        {
            if (points?.Length == 0 || point == null)
                return false;

            for (int pos = 1; pos < points.Length; pos++)
            {
                SlopeAndOffset so = new SlopeAndOffset(points[pos].X, points[pos].Y, points[pos - 1].X, points[pos - 1].Y);

                if (Math.Abs(point.Y - (so.Slope * point.X + so.B)) < Math.Abs(tolerance))
                    return true;
            }

            return false;
        }

        #region Distancia ponto da uma linha, utilizado para validar pedágios em rota...

        /// <summary>
        /// Verifica se um ponto está tocando uma linha de acordo com a tolerância
        /// </summary>
        /// <param name="points">Todos os pontos da linha a ser checados...</param>
        /// <param name="point">Ponto a ser analisado se está tocando a linha.</param>
        /// <param name="tolerance">Tolerância da distância..</param>
        /// <returns></returns>
        public static bool PointTouchLine(PointF[] points, PointF point, double tolerance)
        {
            if (points?.Length == 0 || point == null)
                return false;

            for (int i = 1; i < points.Length; i++)
            {
                double dist = LineToPointDistance2D(points[i - 1], points[i], point, true);
                if (dist < tolerance)
                    return true;
            }
            return false;
        }

        //Compute the dot product AB . BC
        private static double DotProduct(PointF pointA, PointF pointB, PointF pointC)
        {
            PointF AB = new PointF();
            PointF BC = new PointF();
            AB.X = pointB.X - pointA.X;
            AB.Y = pointB.Y - pointA.Y;
            BC.X = pointC.X - pointB.X;
            BC.Y = pointC.Y - pointB.Y;
            double dot = AB.X * BC.X + AB.Y * BC.Y;
            return dot;
        }

        //Compute the cross product AB x AC
        private static double CrossProduct(PointF pointA, PointF pointB, PointF pointC)
        {
            PointF AB = new PointF();
            PointF AC = new PointF();
            AB.X = pointB.X - pointA.X;
            AB.Y = pointB.Y - pointA.Y;
            AC.X = pointC.X - pointA.X;
            AC.Y = pointC.Y - pointA.Y;
            double cross = AB.X * AC.Y - AB.Y * AC.X;
            return cross;
        }

        //Compute the distance from A to B
        private static double Distance(PointF pointA, PointF pointB)
        {
            double d1 = pointA.X - pointB.X;
            double d2 = pointA.Y - pointB.Y;

            return Math.Sqrt(d1 * d1 + d2 * d2);
        }

        //Compute the distance from AB to C
        //if isSegment is true, AB is a segment, not a line.
        private static double LineToPointDistance2D(PointF pointA, PointF pointB, PointF pointC, bool isSegment)
        {
            double dist = CrossProduct(pointA, pointB, pointC) / Distance(pointA, pointB);
            if (isSegment)
            {
                double dot1 = DotProduct(pointA, pointB, pointC);
                if (dot1 > 0)
                    return Distance(pointB, pointC);

                double dot2 = DotProduct(pointB, pointA, pointC);
                if (dot2 > 0)
                    return Distance(pointA, pointC);
            }
            return Math.Abs(dist);
        }

        #endregion
    }

    public class SlopeAndOffset
    {
        public double Slope { get; set; }
        public double B { get; set; }

        public SlopeAndOffset(double x1, double y1, double x2, double y2)
        {
            Slope = (y2 - y1) / (x2 - x1);
            B = y1 - Slope * x1;
        }
    }
}