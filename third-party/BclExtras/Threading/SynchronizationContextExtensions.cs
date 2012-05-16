using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Reflection.Emit;
using BclExtras.Collections;
using System.Reflection;
#if NETFX_35
using System.Linq;
#endif

namespace BclExtras.Threading
{
    /// <summary>
    /// Handy extension methods for a SynchronizationContext which allow for passing of 
    /// more common delegates
    /// </summary>
    public static class SynchronizationContextExtensions
    {
        #region DelegateFactory

        private static class DelegateFactory
        {
            private const string PostString = "Post";
            private const string SendString = "Send";

            private class DelegateData
            {
                private SynchronizationContext m_context;
                private Delegate m_target;

                internal DelegateData(SynchronizationContext context, Delegate target)
                {
                    m_target = target;
                    m_context = context;
                }

                public void Send(object[] args)
                {
                    m_context.Send(() => m_target.DynamicInvoke(args));
                }

                public void Post(object[] args)
                {
                    m_context.Post(() => m_target.DynamicInvoke(args));
                }
            }


            private static T Create<T>(SynchronizationContext context, T target, string name)
            {
                Delegate del = (Delegate)(object)target;
                if (del.Method.ReturnType != typeof(void))
                {
                    throw new ArgumentException("Only void return types currently supported");
                }

                var paramList = new List<Type>();
                paramList.Add(typeof(DelegateData));
                paramList.AddRange(del.Method.GetParameters().Select((x) => x.ParameterType));
                var method = new DynamicMethod(
                    "AMethodName",
                    del.Method.ReturnType,
                    paramList.ToArray(),
                    typeof(DelegateData));
                var gen = method.GetILGenerator();
                var localInfo = gen.DeclareLocal(typeof(object[]));
                gen.Emit(OpCodes.Ldc_I4, paramList.Count - 1);
                gen.Emit(OpCodes.Newarr, typeof(object));
                gen.Emit(OpCodes.Stloc, localInfo.LocalIndex);
                for (int i = 1; i < paramList.Count; ++i)
                {
                    gen.Emit(OpCodes.Ldloc, localInfo.LocalIndex);
                    gen.Emit(OpCodes.Ldc_I4, i - 1);
                    gen.Emit(OpCodes.Ldarg, i);
                    if (paramList[i].IsValueType)
                    {
                        gen.Emit(OpCodes.Box);
                    }
                    gen.Emit(OpCodes.Stelem_Ref);
                }

                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldloc, localInfo.LocalIndex);
                gen.EmitCall(OpCodes.Call, typeof(DelegateData).GetMethod(name, BindingFlags.Instance | BindingFlags.Public), null);
                gen.Emit(OpCodes.Ret);
                return (T)(object)method.CreateDelegate(typeof(T), new DelegateData(context, del));
            }

            internal static T CreateAsSend<T>(SynchronizationContext context, T target)
            {
                return Create(context, target, SendString);
            }

            internal static T CreateAsPost<T>(SynchronizationContext context, T target)
            {
                return Create(context, target, PostString);
            }
        }

        #endregion

        #region Extra Send/Post Methods Typed to better delegates

        public static void Post(this SynchronizationContext context, Action action)
        {
            context.Post((x) => action(), null);
        }

        public static void Post<T>(this SynchronizationContext context, Action<T> action, T arg)
        {
            context.Post(() => action(arg));
        }

        public static void Post<TArg1, TArg2>(this SynchronizationContext context, Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2)
        {
            context.Post(() => action(arg1, arg2));
        }

        public static void Send(this SynchronizationContext context, Action action)
        {
            context.Send((x) => action(), null);
        }

        public static void Send<T>(this SynchronizationContext context, Action<T> action, T arg)
        {
            context.Send(() => action(arg));
        }

        public static void Send<TArg1, TArg2>(this SynchronizationContext context, Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2)
        {
            context.Send(() => action(arg1, arg2));
        }

        #endregion

        #region Higher order binding

        public static Action BindAsSend(this SynchronizationContext context, Action action)
        {
            return () => context.Send(action);
        }

        public static Action<TArg1> BindAsSend<TArg1>(this SynchronizationContext context, Action<TArg1> action)
        {
            return (arg1) => context.Send(() => action(arg1));
        }

        public static Action<TArg1, TArg2> BindAsSend<TArg1, TArg2>(this SynchronizationContext context, Action<TArg1, TArg2> action)
        {
            return (arg1, arg2) => context.Send(() => action(arg1, arg2));
        }

        public static Action BindAsPost(this SynchronizationContext context, Action action)
        {
            return () => context.Post(action);
        }

        public static Action<TArg1> BindAsPost<TArg1>(this SynchronizationContext context, Action<TArg1> action)
        {
            return (arg1) => context.Post(() => action(arg1));
        }

        public static Action<TArg1, TArg2> BindAsPost<TArg1, TArg2>(this SynchronizationContext context, Action<TArg1, TArg2> action)
        {
            return (arg1, arg2) => context.Post(() => action(arg1, arg2));
        }

        public static T BindDelegateAsPost<T>(this SynchronizationContext context, T del)
        {
            return DelegateFactory.CreateAsPost(context, del);
        }

        public static T BindDelegateAsSend<T>(this SynchronizationContext context, T del)
        {
            return DelegateFactory.CreateAsSend(context, del);
        }


        #endregion

    }
}
