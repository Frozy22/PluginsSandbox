using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine.Assertions;
using UnityEngine.Windows;

namespace FrozenBox.Utils.Editor
{
    public static class AssemblyUtils
    {
        public static void CreateAssembly(string inFolder, string assemblyName, bool isEditorAssembly, params string[]? referencedAssemblies)
        {
            Assert.IsTrue(AssetDatabase.IsValidFolder(inFolder), "Invalid folder path.");
            Assert.IsFalse(string.IsNullOrEmpty(assemblyName), "Invalid assembly name.");
            
            var assemblyPath = $"{inFolder}/{assemblyName}.asmdef";
            var referenceContent = referencedAssemblies != null ? $"\"references\": [{string.Join(",\n", referencedAssemblies.Select(name => $"\"{name}\""))}]" : string.Empty;
            var content = isEditorAssembly
                ? $"{{\n  \"name\": \"{assemblyName}\",\n  \"rootNamespace\": \"{assemblyName}\",\n  {referenceContent},\n  \"includePlatforms\": [ \"Editor\" ]\n}}"
                : $"{{\n  \"name\": \"{assemblyName}\",\n  \"rootNamespace\": \"{assemblyName}\",\n  {referenceContent}\n}}";
            
            File.WriteAllBytes(assemblyPath, Encoding.ASCII.GetBytes(content));
            AssetDatabase.ImportAsset(assemblyPath);
        }
    }
}