using System.Linq;
using UnityEditor;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

public static class InjectTool {

    private const string AssemblyPath = "./Library/ScriptAssemblies/Assembly-CSharp.dll";

    [MenuItem("Custom/Inject")]
    public static void Inject()
    {
        if (Application.isPlaying || EditorApplication.isCompiling)
        {
            Debug.Log("You need stop play mode or wait compiling finished");
            return;
        }

        // 加载程序集
        var assembly = AssemblyDefinition.ReadAssembly(AssemblyPath);
        if (assembly == null)
        {
            Debug.LogFormat("Can't load assembly:{0}", AssemblyPath);
            return;
        }

        var module = assembly.MainModule;
        foreach (var type in module.Types)
        {
            // 找到module中需要注入的类型
            var needInjectAttr = typeof(TestInjectAttribute).FullName;
            var needInject = type.CustomAttributes.Any(customAttribute => customAttribute.GetType().FullName == needInjectAttr);
            if (!needInject)
            {
                continue;
            }
            // 只对公有方法进行注入
            foreach (var method in type.Methods)
            {
                if (method.IsConstructor || method.IsGetter || method.IsSetter || !method.IsPublic)
                {
                    continue;
                }
                InjectMethod(module, method);
            }
        }
        assembly.Write(AssemblyPath, new WriterParameters {WriteSymbols = true});
    }

    private static void InjectMethod(ModuleDefinition module, MethodDefinition method)
    {
        // 定义稍后会用的类型
        var objType = module.Import(typeof(System.Object));
        var intType = module.Import(typeof(System.Int32));
        var logFormatMethod = module.Import(typeof(Debug).GetMethod("LogFormat"));
        // 开始注入IL代码
        var insertPoint = method.Body.Instructions[0];
        var ilProcessor = method.Body.GetILProcessor();
        // 设置一些标签用于语句跳转
        var label1 = ilProcessor.Create(OpCodes.Ldarg_1);
        var label2 = ilProcessor.Create(OpCodes.Stloc_0);
        var label3 = ilProcessor.Create(OpCodes.Stloc_0);
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldstr, "a = {0}, b = {1}"));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4_2));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Newarr, objType));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Dup));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Box, intType));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stelem_Ref));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Dup));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4_1));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_1));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Box, intType));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stelem_Ref));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Call, logFormatMethod));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_1));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ble, label1));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Br, label2));
        ilProcessor.InsertBefore(insertPoint, label1);
        ilProcessor.InsertBefore(insertPoint, label2);
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Br, label3));
        ilProcessor.InsertBefore(insertPoint, label3);
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ret));
    }
}
