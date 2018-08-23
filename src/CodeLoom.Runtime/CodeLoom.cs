using CodeLoom.Aspects;
using CodeLoom.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeLoom
{
    public static class CodeLoom
    {
        private static readonly object INSTANCE_ASPECTS_LOCK = new object();
        private static readonly object STATIC_ASPECTS_LOCK = new object();
        private static readonly ConditionalWeakTable<Type, List<IAspect>> STATIC_ASPECTS = new ConditionalWeakTable<Type, List<IAspect>>();
        private static readonly ConditionalWeakTable<object, List<IAspect>> INSTANCE_ASPECTS = new ConditionalWeakTable<object, List<IAspect>>();

        public static void AddAspect(object target, IAspect aspect)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (aspect == null)
                throw new ArgumentNullException(nameof(aspect));

            var aspectsList = GetAspectsList(target);

            if (aspectsList == null)
                throw new InvalidOperationException($"Cannot find instance aspects list on type {target.GetType().FullName}. Check if the type has been properly weaved.");

            lock (INSTANCE_ASPECTS_LOCK) { aspectsList.Add(aspect); }
        }

        public static void RemoveAspect(object target, IAspect aspect)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (aspect == null)
                throw new ArgumentNullException(nameof(aspect));

            var aspectsList = GetAspectsList(target);

            if (aspectsList == null)
                throw new InvalidOperationException($"Cannot find instance aspects list on type {target.GetType().FullName}. Check if the type has been properly weaved.");

            lock (INSTANCE_ASPECTS_LOCK) { aspectsList.Remove(aspect); }
        }

        public static void ConfigureAspects(object target, Action<IList<IAspect>> configure)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var aspectsList = GetAspectsList(target);

            if (aspectsList == null)
                throw new InvalidOperationException($"Cannot find instance aspects list on type {target.GetType().FullName}. Check if the type has been properly weaved.");

            lock (INSTANCE_ASPECTS_LOCK) { configure(aspectsList); }
        }

        public static void AddAspect<T>(IAspect aspect)
        {
            AddAspect(typeof(T), aspect);
        }

        public static void AddAspect(Type type, IAspect aspect)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (aspect == null)
                throw new ArgumentNullException(nameof(aspect));

            var aspectsList = GetAspectsList(type);

            if (aspectsList == null)
                throw new InvalidOperationException($"Cannot find static aspects list on type {type.FullName}. Check if the type has been properly weaved.");

            lock (INSTANCE_ASPECTS_LOCK) { aspectsList.Add(aspect); }
        }

        public static void RemoveAspect<T>(IAspect aspect)
        {
            RemoveAspect(typeof(T), aspect);
        }

        public static void RemoveAspect(Type type, IAspect aspect)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (aspect == null)
                throw new ArgumentNullException(nameof(aspect));

            var aspectsList = GetAspectsList(type);

            if (aspectsList == null)
                throw new InvalidOperationException($"Cannot find static aspects list on type {type.FullName}. Check if the type has been properly weaved.");

            lock (INSTANCE_ASPECTS_LOCK) { aspectsList.Remove(aspect); }
        }

        public static void ConfigureAspects<T>(Action<IList<IAspect>> configure)
        {
            ConfigureAspects(typeof(T), configure);
        }

        public static void ConfigureAspects(Type type, Action<IList<IAspect>> configure)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var aspectsList = GetAspectsList(type);

            if (aspectsList == null)
                throw new InvalidOperationException($"Cannot find static aspects list on type {type.FullName}. Check if the type has been properly weaved.");

            lock (INSTANCE_ASPECTS_LOCK) { configure(aspectsList); }
        }

        public static void Invoke(Invocation invocation)
        {
            var staticAspects = GetAspectsList(invocation.Source.Type, false);
            var instanceAspects = GetAspectsList(invocation.Source.Instance, false);
            var aspectWasInvoked = false;

            if (staticAspects != null)
            {
                foreach (var aspect in staticAspects)
                {
                    aspect.Execute(invocation);
                    aspectWasInvoked = true;
                }
            }

            if (instanceAspects != null)
            {
                foreach (var aspect in instanceAspects)
                {
                    aspect.Execute(invocation);
                    aspectWasInvoked = true;
                }
            }

            if (!aspectWasInvoked)
            {
                invocation.Proceed();
            }
        }

        private static IList<IAspect> GetAspectsList(object target, bool addIfDoesNotExists = true)
        {
            if (target == null) return null;

            if(addIfDoesNotExists)
            {
                List<IAspect> list = INSTANCE_ASPECTS.GetOrCreateValue(target);
                return list;
            }
            else
            {
                List<IAspect> list = null;
                INSTANCE_ASPECTS.TryGetValue(target, out list);
                return list;
            }
        }

        private static IList<IAspect> GetAspectsList(Type type, bool addIfDoesNotExists = true)
        {
            if (type == null) return null;

            if (addIfDoesNotExists)
            {
                List<IAspect> list = STATIC_ASPECTS.GetOrCreateValue(type);
                return list;
            }
            else
            {
                List<IAspect> list = null;
                STATIC_ASPECTS.TryGetValue(type, out list);
                return list;
            }
        }
    }
}
