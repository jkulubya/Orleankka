﻿using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Orleankka.Features.Actor_behaviors
{
    namespace Reusing_handlers_via_traits
    {
        using Behaviors;

        [TestFixture]
        class Tests
        {
            class X {}
            class Y {}

            List<string> events;

            void AssertEvents(params string[] expected) => 
                CollectionAssert.AreEqual(expected, events);

            [SetUp]
            public void SetUp() => 
                events = new List<string>();

            [Test]
            public async Task When_trait_handles_message()
            {
                Receive @base = message =>
                {
                    events.Add("base");
                    return TaskResult.Unhandled;
                };

                Task<object> XTrait(object message)
                {
                    events.Add("x");
                    return TaskResult.Unhandled;
                }

                Task<object> YTrait(object message)
                {
                    events.Add("y");
                    return TaskResult.From("y");
                }

                var receive = @base.Trait(XTrait, YTrait);
                var result = await receive("foo");

                AssertEqual(new[] {"base", "x", "y"}, events);
                Assert.AreEqual("y", result);
            }

            [Test]
            public async Task When_base_handles_message()
            {
                Receive @base = message =>
                {
                    events.Add("base");
                    return TaskResult.From("base");
                };

                Task<object> XTrait(object message)
                {
                    events.Add("x");
                    return TaskResult.From("x");
                }

                Task<object> YTrait(object message)
                {
                    events.Add("y");
                    return TaskResult.From("y");
                }

                var receive = @base.Trait(XTrait, YTrait);
                var result = await receive("foo");

                AssertEqual(new[] {"base"}, events);
                Assert.AreEqual("base", result);
            }

            [Test]
            public async Task When_none_of_receives_handles_message()
            {
                Receive @base = message =>
                {
                    events.Add("base");
                    return TaskResult.Unhandled;
                };

                Task<object> XTrait(object message)
                {
                    events.Add("x");
                    return TaskResult.Unhandled;
                }

                Task<object> YTrait(object message)
                {
                    events.Add("y");
                    return TaskResult.Unhandled;
                }

                var receive = @base.Trait(XTrait, YTrait);
                var result = await receive("foo");

                AssertEqual(new[] {"base", "x", "y"}, events);
                Assert.AreSame(Unhandled.Result, result);
            }

            [Test]
            public async Task When_handling_lifecycle_events()
            {
                Receive @base = message =>
                {
                    events.Add("base");
                    return TaskResult.Done;
                };

                Task<object> XTrait(object message)
                {
                    events.Add("x");
                    return TaskResult.Unhandled;
                }

                Task<object> YTrait(object message)
                {
                    events.Add("y");
                    return TaskResult.Done;
                }

                var receive = @base.Trait(XTrait, YTrait);
                await receive(Activate.Message);
                await receive(Deactivate.Message);

                AssertEqual(new[] {"y", "x", "base", "y", "x", "base"}, events);
            }

            static void AssertEqual(IEnumerable<string> expected, IEnumerable<string> actual) =>
                CollectionAssert.AreEqual(expected, actual);
        }
    }
}