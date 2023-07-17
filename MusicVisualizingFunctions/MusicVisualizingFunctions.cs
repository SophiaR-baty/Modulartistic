using System;

namespace MusicVisualizingFunctions
{
    public static class MusicVisualizingFunctions
    {
        public static double Equalizer(double x, double y, double off_x, double off_y, double girth, double len, double strength, double max, double active_out, double inactive_out)
        {
            // setting the valid rectangle bounds
            double xmin, xmax, ymin, ymax;
            xmin = off_x - girth / 2;
            xmax = off_x + girth / 2;
            ymin = off_y;
            ymax = ymin + len*strength/max;

            if (ymin > ymax) { ymin = ymax; ymin = off_y; }
            if (x >= xmin && x <= xmax && y >= ymin && y <= ymax)
            {
                return active_out;
            }
            else
            {
                return inactive_out;
            }
        }
    }
}
