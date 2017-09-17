using System;
using Game.Component;
using UnityEngine;
namespace Game
{
    public class Bitmask
    {
        private UInt64 _bits;

        private Bitmask(UInt64 bits)
        {
            _bits = bits;
        }

        public UInt64 Value { get { return _bits; } }
        public bool Fits(Bitmask theLock)
        {
            return (theLock._bits & _bits) == _bits;
        }

        public void Set(int bit)
        {
            _bits |= 1UL << bit;
        }

        public void Unset(int bit)
        {
            _bits &= ~(1UL << bit);
        }

        public bool IsSet(int bit)
        {
            return (_bits & (1UL << bit)) == (1UL << bit);
        }

        public static Bitmask Zero
        {
            get
            {
                return new Bitmask(0);
            }
        }

        public Bitmask Copy()
        {
            return new Bitmask(_bits);
        }

        public override string ToString()
        {
            return Convert.ToString(_bits);
        }

        private static UInt64 BitValueOf<T>()
            where T : GComponent
        {
            return 1UL << GComponent.GetID<T>();
        }

        #region MakeFromComponents<...>()
        public static Bitmask MakeFromComponents<T0>() where T0 : GComponent
        {
            UInt64 bitval = 0;
            bitval |= BitValueOf<T0>();
            return new Bitmask(bitval);
        }

        public static Bitmask MakeFromComponents<T0, T1>()
            where T0 : GComponent
            where T1 : GComponent
        {
            UInt64 bitval = 0;
            bitval |= BitValueOf<T0>();
            bitval |= BitValueOf<T1>();
            return new Bitmask(bitval);
        }

        public static Bitmask MakeFromComponents<T0, T1, T2>()
            where T0 : GComponent
            where T1 : GComponent
            where T2 : GComponent
        {
            UInt64 bitval = 0;
            bitval |= BitValueOf<T0>();
            bitval |= BitValueOf<T1>();
            bitval |= BitValueOf<T2>();
            return new Bitmask(bitval);
        }

        public static Bitmask MakeFromComponents<T0, T1, T2, T3>()
            where T0 : GComponent
            where T1 : GComponent
            where T2 : GComponent
            where T3 : GComponent
        {
            UInt64 bitval = 0;
            bitval |= BitValueOf<T0>();
            bitval |= BitValueOf<T1>();
            bitval |= BitValueOf<T2>();
            bitval |= BitValueOf<T3>();
            return new Bitmask(bitval);
        }

        public static Bitmask MakeFromComponents<T0, T1, T2, T3, T4>()
            where T0 : GComponent
            where T1 : GComponent
            where T2 : GComponent
            where T3 : GComponent
            where T4 : GComponent
        {
            UInt64 bitval = 0;
            bitval |= BitValueOf<T0>();
            bitval |= BitValueOf<T1>();
            bitval |= BitValueOf<T2>();
            bitval |= BitValueOf<T3>();
            bitval |= BitValueOf<T4>();
            return new Bitmask(bitval);
        }

        public static Bitmask MakeFromComponents<T0, T1, T2, T3, T4, T5>()
            where T0 : GComponent
            where T1 : GComponent
            where T2 : GComponent
            where T3 : GComponent
            where T4 : GComponent
            where T5 : GComponent
        {
            UInt64 bitval = 0;
            bitval |= BitValueOf<T0>();
            bitval |= BitValueOf<T1>();
            bitval |= BitValueOf<T2>();
            bitval |= BitValueOf<T3>();
            bitval |= BitValueOf<T4>();
            bitval |= BitValueOf<T5>();
            return new Bitmask(bitval);
        }

        public static Bitmask MakeFromComponents<T0, T1, T2, T3, T4, T5, T6>()
            where T0 : GComponent
            where T1 : GComponent
            where T2 : GComponent
            where T3 : GComponent
            where T4 : GComponent
            where T5 : GComponent
            where T6 : GComponent
        {
            UInt64 bitval = 0;
            bitval |= BitValueOf<T0>();
            bitval |= BitValueOf<T1>();
            bitval |= BitValueOf<T2>();
            bitval |= BitValueOf<T3>();
            bitval |= BitValueOf<T4>();
            bitval |= BitValueOf<T5>();
            bitval |= BitValueOf<T6>();
            return new Bitmask(bitval);
        }

        public static Bitmask MakeFromComponents<T0, T1, T2, T3, T4, T5, T6, T7>()
            where T0 : GComponent
            where T1 : GComponent
            where T2 : GComponent
            where T3 : GComponent
            where T4 : GComponent
            where T5 : GComponent
            where T6 : GComponent
            where T7 : GComponent
        {
            UInt64 bitval = 0;
            bitval |= BitValueOf<T0>();
            bitval |= BitValueOf<T1>();
            bitval |= BitValueOf<T2>();
            bitval |= BitValueOf<T3>();
            bitval |= BitValueOf<T4>();
            bitval |= BitValueOf<T5>();
            bitval |= BitValueOf<T6>();
            bitval |= BitValueOf<T7>();
            return new Bitmask(bitval);
        }

        #endregion
    }
}
