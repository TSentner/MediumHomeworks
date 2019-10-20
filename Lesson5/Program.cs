using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _12_Builder
{
    class Program
    {
        static void Main(string[] args)
        {
            var unit = UnitBuilder.Start().WithPhysicalArmor().MovingOnPath().Create();
        }
    }


    class UnitBuilder
    {
        private GameObject _gameObject;
        private IArmor _armor;

        private UnitBuilder()
        {
            _gameObject = new GameObject();
        }

        public interface IUnitArmorBuilder
        {
            IUnitMoveOnGroundBuilder WithPhysicalArmor();
            IUnitMoveBuilder WithMagicalArmor();
        }

        public interface IUnitMoveOnGroundBuilder
        {
            IUnitCreateBuilder MovingOnPath();
        }

        public interface IUnitMoveBuilder
        {
            IUnitCreateBuilder MovingOnPath();
            IUnitCreateBuilder Flying();
        }

        public interface IUnitCreateBuilder
        {
            GameObject Create();
        }

        public static IUnitArmorBuilder Start() => new UnitArmorBuilder(new UnitBuilder());

        private class UnitArmorBuilder : IUnitArmorBuilder
        {
            private UnitBuilder _unitBuilder;

            public UnitArmorBuilder(UnitBuilder unitBuilder)
            {
                _unitBuilder = unitBuilder;
            }

            public IUnitMoveOnGroundBuilder WithPhysicalArmor()
            {
                _unitBuilder._armor = new PhysicalArmor();
                return new UnitMoveBuilder(_unitBuilder);
            }

            public IUnitMoveBuilder WithMagicalArmor()
            {
                _unitBuilder._armor = new MagicArmor();
                return new UnitMoveBuilder(_unitBuilder);
            }
        }

        private class UnitMoveBuilder : IUnitMoveBuilder, IUnitMoveOnGroundBuilder
        {
            private UnitBuilder _unitBuilder;

            public UnitMoveBuilder(UnitBuilder unitBuilder)
            {
                _unitBuilder = unitBuilder;
            }

            public IUnitCreateBuilder MovingOnPath()
            {
                _unitBuilder._gameObject.Components.Add(new PathMovement());
                return new UnitCreateBuilder(_unitBuilder);
            }

            public IUnitCreateBuilder Flying()
            {
                _unitBuilder._gameObject.Components.Add(new FlyMovement());
                return new UnitCreateBuilder(_unitBuilder);
            }
        }

        private class UnitCreateBuilder : IUnitCreateBuilder
        {
            private UnitBuilder _unitBuilder;

            public UnitCreateBuilder(UnitBuilder unitBuilder)
            {
                _unitBuilder = unitBuilder;
            }

            public GameObject Create()
            {
                _unitBuilder._gameObject.Components.Add(new Health()
                {
                    Armor = _unitBuilder._armor
                });

                return _unitBuilder._gameObject;
            }
        }
    }


    class GameObject
    {
        public List<IComponent> Components;
    }

    interface IComponent
    {
        void Start();
        void Update();
    }

    class Health : IComponent
    {
        public IArmor Armor;
    }

    interface IArmor
    {
        int ProccessDamage(int damage);
    }

    class MagicArmor : IArmor
    {
        
    }

    class PhysicalArmor : IArmor
    {

    }

    abstract class Movement : IComponent
    {

    }

    class PathMovement : Movement
    {

    }

    class FlyMovement : Movement
    {

    }

}