using System;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace FrozenBox.TagsSystem
{
    public static partial class TagEnumsRegistry
    {
        private static readonly HashSet<Type> Types = new();
        
        public static bool Contains<T>() where T : Enum => Types.Contains(typeof(T));
        public static bool Contains(Type type) => Types.Contains(type);

        public static void RegisterType<T>() where T : Enum 
            => Types.Add(typeof(T));

        [OnCodeInitializing]
        private static void DefaultRegistries()
        {
            RegisterType<Space>();
            RegisterType<ForceMode>();
            RegisterType<ForceMode2D>();
        }
    }
}