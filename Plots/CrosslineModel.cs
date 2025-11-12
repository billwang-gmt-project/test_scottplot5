using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Plots
{
    public class CrosslineModel
    {
        public VerticalLine vLine = new()
        {
            // Make the lines draggable
            IsDraggable = true,
            // hide cross line
            IsVisible = false,
        };

        public HorizontalLine hLine = new()
        {
            // Make the lines draggable
            IsDraggable = true,
            // hide cross line
            IsVisible = false,
        };

        public Annotation? xAnnotation;
        public Annotation? yAnnotation;

        public DataPoint? point = new();
        public Coordinates? coordinates;

        /// <summary>
        /// Text can be placed at data points
        /// </summary>
        public Text? xText;
        public Text? yText;
    }
}
