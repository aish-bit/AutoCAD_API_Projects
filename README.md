Polygon Node Generator - AutoCAD Plugin

==> Project Overview This AutoCAD plugin generates nodes along the edges of a selected polygon (closed polyline) at a uniform spacing of 1 meter. If a generated node falls beyond the edge, it is adjusted to fit within the edge boundaries. And if node falls at a corner, then skip it. extra : Check if node is too close to the end point then skip it (not mentioned in assignment)

==> Environment Details Software: AutoDesk AutoCAD 2025 (Version: 25.0.154.0) Platform: Windows 11 .NET Framework: .NET 8.0 Development IDE: Visual Studio 2022 Community Edition

==> Installation & Setup 1. Build the Project -> Open the solution in Visual Studio 2022. -> Set the target framework to .NET 8.0. - > Add references of dll (Note : currently manually local reference added in project. So while building project need to resolve them at local environment) # accoremgd.dll # acdbmgd.dll # acmgd.dll

==> Build the solution to generate the DLL file. (currently trial happen in : Debug(Any CPU) mode)

==> Load the Plugin in AutoCAD Open AutoCAD 2025. Open the Command Line and type: NETLOAD

==> Browse to the folder containing the built DLL file and select it. The plugin is now loaded into AutoCAD.

==> Usage Instructions

1. Display Greeting Message
	Command: HELLO_Human
	Output: Displays "Hello Aishwarya!!!" in the AutoCAD console.

2. Generate Nodes Along Polygon Edges
	Command: Polygon_Node_Generator
	Steps:
		Run the command in AutoCAD.
		Select a closed polyline (polygon) when prompted.
		The program calculates 1-meter spaced nodes, adjusting for corner placements.
		Nodes are displayed as points (DBPoint) in the drawing.
		The command completes with a success message.


Key Features
✔ Adjusts node placement to ensure all nodes remain within polygon edges.
✔ Supports different AutoCAD drawing units (automatically converts to meters).
✔ Uses AutoCAD's system variables to ensure visibility of generated nodes.
==> Troubleshooting NETLOAD command not working? Ensure AutoCAD is running in elevated mode (Run as Administrator). Polyline not detected? Ensure the selected object is a closed polygon created using polyline. Nodes not visible? Use the command: PDMODE : set it to 3, then run REGEN : to refresh the drawing. Currently references of dll manually local reference added in project. So while building project need to resolve them at local environment # accoremgd.dll # acdbmgd.dll # acmgd.dll

==> References used : AutoCAD .NET API Documentation: Autodesk Developer Guide AutoCAD 2025 System Requirements: Autodesk Support AI tool : ex. chat gpt AutoDesk Forum https://www.youtube.com/watch?v=4vCoNR8VKH8&list=PLtUgMNwS-a42A4O6QSoHhI63vgCSpkYYt AutoCAD 2025 System requirements : https://www.autodesk.com/support/technical/article/caas/sfdcarticles/sfdcarticles/System-requirements-for-AutoCAD-2025-including-Specialized-Toolsets.html Developer use : https://help.autodesk.com/view/OARX/2025/ENU/

==> Note : Currently the project is at a very initial level (Level 1) and will need further polishing to follow all coding practices. I have implemented the basic functionality as requested in the assignment. Need further improvement.
