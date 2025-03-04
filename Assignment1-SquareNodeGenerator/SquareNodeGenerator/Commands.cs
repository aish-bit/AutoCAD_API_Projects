/******Polygon Node Generator - AutoCAD Plugin******/
/*Currently the project is at a very initial level (Level 1) and will need further polishing to follow all coding practices. I have implemented the basic functionality as requested in the assignment. Need further improvement.*/

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

[assembly: CommandClass(typeof(SquareNodeGenerator.Commands))]

namespace SquareNodeGenerator
{
    public class Commands
    {
        [CommandMethod("HELLO_Human")]
        public void HelloWorld()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            ed.WriteMessage("Hello Aishwarya!!!");
        }

        [CommandMethod("Polygon_Node_Generator")]
        public void PolygonNodeGenerator()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            try
            {
                // Set PDMODE and PDSIZE to make points visible
                Application.SetSystemVariable("PDMODE", 3);  // X-Shaped Points
                Application.SetSystemVariable("PDSIZE", 0.5); // Adjust size
                ed.Regen(); // Refresh the view

                // Ask user to select a polygon (closed polyline)
                PromptEntityOptions peo = new PromptEntityOptions("\nSelect a polygon (closed polyline): ");
                peo.SetRejectMessage("\nSelected object is not a closed polyline.");
                peo.AddAllowedClass(typeof(Polyline), true);

                PromptEntityResult per = ed.GetEntity(peo);
                if (per.Status != PromptStatus.OK) return;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Polyline polygon = tr.GetObject(per.ObjectId, OpenMode.ForRead) as Polyline;
                    if (polygon == null || !polygon.Closed)
                    {
                        ed.WriteMessage("\nSelected object is not a valid polygon.");
                        return;
                    }

                    // Get AutoCAD Drawing Units
                    object insUnitsObj = Application.GetSystemVariable("INSUNITS");
                    if (insUnitsObj == null)
                    {
                        ed.WriteMessage("\nCould not retrieve drawing units.");
                        return;
                    }

                    if (!short.TryParse(insUnitsObj.ToString(), out short insUnitsShort))
                    {
                        ed.WriteMessage("\nInvalid drawing units setting.");
                        return;
                    }

                    UnitsValue fromUnit = (UnitsValue)insUnitsShort;
                    double unitToMeter = GetUnitConversionFactor(fromUnit);

                    if (unitToMeter <= 0)
                    {
                        ed.WriteMessage("\nUnsupported drawing unit. Cannot calculate spacing.");
                        return;
                    }

                    ed.WriteMessage($"\nConversion Factor: 1 Unit = {unitToMeter} Meters");


                    List<Point3d> nodePoints = new List<Point3d>();
                    double nodeSpacing = 1.0; // Nodes at 1-sq meter unit distance

                    for (int i = 0; i < polygon.NumberOfVertices; i++)
                    {
                        Point3d start = polygon.GetPoint3dAt(i);
                        Point3d end = polygon.GetPoint3dAt((i + 1) % polygon.NumberOfVertices);

                        double edgeLength = start.DistanceTo(end) * unitToMeter; // Convert to meters

                        if (edgeLength <= nodeSpacing) { continue; } // Skip edges too short for nodes

                        int numNodes = (int)(edgeLength / nodeSpacing); // Count nodes at 1m spacing

                        for (int j = 1; j <= numNodes; j++) // Start from 1 to avoid duplicating start vertex
                        {
                            double distance = j * nodeSpacing; // Exact spacing for each node

                            // if the node falls at the corner, skip it
                            if (distance == edgeLength)
                                { continue; }
                                // If the node falls beyond the edge, adjust it to fit inside
                            else   if (distance > edgeLength)
                            {
                                if (edgeLength - (j - 1) * nodeSpacing > 0.5) // If gap is large, adjust the last node
                                    distance = edgeLength - 0.1; // Push slightly inside the edge
                                else
                                    break; // Otherwise, skip this node
                            }

                            double t = distance / edgeLength; // Parameter for interpolation

                            Point3d node = new Point3d(
                                start.X + (end.X - start.X) * t,
                                start.Y + (end.Y - start.Y) * t,
                                start.Z + (end.Z - start.Z) * t
                            );

                            // Check if node is too close to the end point (corner)
                            if (node.DistanceTo(end) < 0.01 * unitToMeter) { continue; } // Skip if too close to the corner

                            nodePoints.Add(node);
                        }
                    }

                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    foreach (var node in nodePoints)
                    {
                        using (DBPoint point = new DBPoint(node))
                        {
                            btr.AppendEntity(point);
                            tr.AddNewlyCreatedDBObject(point, true);
                        }
                    }

                    tr.Commit();
                    ed.WriteMessage("\nNodes created successfully!");
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\nError: {ex.Message}");
            }
        }


        private double GetUnitConversionFactor(UnitsValue fromUnit)
        {
            return fromUnit switch
            {
                UnitsValue.Inches => 0.0254,
                UnitsValue.Feet => 0.3048,
                UnitsValue.Miles => 1609.344,
                UnitsValue.Millimeters => 0.001,
                UnitsValue.Centimeters => 0.01,
                UnitsValue.Meters => 1.0,
                UnitsValue.Kilometers => 1000.0,
                UnitsValue.MicroInches => 0.0000000254,
                UnitsValue.Mils => 0.0000254,
                UnitsValue.Yards => 0.9144,
                UnitsValue.Angstroms => 1.0e-10,
                UnitsValue.Nanometers => 1.0e-9,
                UnitsValue.Microns => 1.0e-6,
                UnitsValue.Decimeters => 0.1,
                UnitsValue.Dekameters => 10.0,
                UnitsValue.Hectometers => 100.0,
                UnitsValue.Gigameters => 1.0e9,
                UnitsValue.Astronomical => 1.495978707e11,
                UnitsValue.LightYears => 9.4607e15,
                UnitsValue.Parsecs => 3.0857e16,
                UnitsValue.USSurveyFeet => 0.3048006096,
                UnitsValue.USSurveyInch => 0.0254000508,
                UnitsValue.USSurveyYard => 0.9144018288,
                UnitsValue.USSurveyMile => 1609.3472,
                _ => 1.0,
            };
        }
    }
}
