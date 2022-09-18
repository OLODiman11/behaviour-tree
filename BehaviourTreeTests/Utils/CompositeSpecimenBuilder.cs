using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
using BehaviourTree.Composites;

namespace BehaviourTreeTests.Utils
{
    public class CompositeSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var t = request as Type;
            if (typeof(Composite) == t)
                return new Fixture().Customize(new AutoNSubstituteCustomization()).Create<Composite>();

            return new NoSpecimen();
        }
    }
}