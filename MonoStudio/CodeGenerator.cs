using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;


namespace MonoStudio
{

    public static class CodeGenerator
    {
        private const string SavePath = "GeneratedCode.txt";

        public static void UpdateCameraCode(Vector2 cameraPosition, Vector2 movement)
        {
            // Build code for camera setup
            var sb = new StringBuilder();
            sb.AppendLine("// Dynamic Camera Setup");
            sb.AppendLine($"var cameraPosition = new Vector2({cameraPosition.X}f, {cameraPosition.Y}f);");
            sb.AppendLine($"var cameraMovement = new Vector2({movement.X}f, {movement.Y}f);");

            // Save to file (overwriting camera part only)
            File.WriteAllText(SavePath, sb.ToString());
        }

        public static void GenerateObjectCode(GameObject gameObject)
        {
            // Generate code for the specified object
            var sb = new StringBuilder();
            sb.AppendLine($"// Code for {gameObject.ObjectName}");
            sb.AppendLine($"var {gameObject.ObjectName.ToLower()} = new {gameObject.ObjectName}();");
            sb.AppendLine($"{gameObject.ObjectName.ToLower()}.Position = new Vector2({gameObject.Position.X}f, {gameObject.Position.Y}f);");
            sb.AppendLine("gameWorld.AddObject(" + gameObject.ObjectName.ToLower() + ");");
            sb.AppendLine();

            // Append to file
            File.AppendAllText(SavePath, sb.ToString());
        }

        public static void UpdateAllCode(IEnumerable<GameObject> gameObjects, Vector2 cameraPosition, Vector2 movement)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// Generated Code - Updated");

            // Update camera information
            sb.AppendLine("// Camera Settings");
            sb.AppendLine($"var cameraPosition = new Vector2({cameraPosition.X}f, {cameraPosition.Y}f);");
            sb.AppendLine($"var cameraMovement = new Vector2({movement.X}f, {movement.Y}f);");
            sb.AppendLine();

            // Update each game object’s position
            foreach (var obj in gameObjects)
            {
                sb.AppendLine($"// Object: {obj.ObjectName}");
                sb.AppendLine($"var {obj.ObjectName.ToLower()} = new {obj.ObjectName}();");
                sb.AppendLine($"{obj.ObjectName.ToLower()}.Position = new Vector2({obj.Position.X}f, {obj.Position.Y}f);");
                sb.AppendLine("gameWorld.AddObject(" + obj.ObjectName.ToLower() + ");");
                sb.AppendLine();
            }

            // Write to file (overwrite entire file with updated code)
            File.WriteAllText(SavePath, sb.ToString());
        }
    }

}
