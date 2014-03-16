using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miq.Tests.Learning
{
    /// From: http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx
    ///
    /// Guideline: the integer returned by GetHashCode should never change
    ///
    /// Ideally, the hash code of a mutable object should be computed from only fields which
    /// cannot mutate, and therefore the hash value of an object is the same for its entire
    /// lifetime.
    ///
    /// However, this is only an ideal-situation guideline; the actual rule is:
    ///
    /// Rule: the integer returned by GetHashCode must never change while the object is contained
    /// in a data structure that depends on the hash code remaining stable
    ///
    /// It is permissible, though dangerous, to make an object whose hash code value can mutate as
    /// the fields of the object mutate. If you have such an object and you put it in a hash table
    /// then the code which mutates the object and the code which maintains the hash table are
    /// required to have some agreed-upon protocol that ensures that the object is not mutated
    /// while it is in the hash table. What that protocol looks like is up to you.
    ///
    /// If an object's hash code can mutate while it is in the hash table then clearly the
    /// Contains method stops working. You put the object in bucket #5, you mutate it, and when
    /// you ask the set whether it contains the mutated object, it looks in bucket #74 and doesn't
    /// find it.
    ///
    /// Remember, objects can be put into hash tables in ways that you didn't expect. A lot of the
    /// LINQ sequence operators use hash tables internally. Don't go dangerously mutating objects
    /// while enumerating a LINQ query that returns them!


    [TestClass]
    public class GetHashCode
    {
        class AClassWithGetHashCodeThatChanges
        {
            public string Id { get; set; }
            public int OtherProperty { get; set; }

            public override int GetHashCode()
            {
                int hashCode = 17;
                hashCode = hashCode * 23 + Id.GetHashCode();
                hashCode = hashCode * 23 + OtherProperty.GetHashCode();
                return hashCode;
            }
        }

        [TestMethod]
        public void BadClass_GetHashCode_Changes()
        {
            var obj = new AClassWithGetHashCodeThatChanges() { Id = "A", OtherProperty = 10 };
            int hashCode1 = obj.GetHashCode();
            obj.OtherProperty = 100;
            int hashCode2 = obj.GetHashCode();

            Assert.AreNotEqual(hashCode1, hashCode2);
        }

        [TestMethod]
        public void BadGetHashCode_ExampleOfProblem()
        {
            var set = new HashSet<AClassWithGetHashCodeThatChanges>();
            var obj = new AClassWithGetHashCodeThatChanges() { Id = "A", OtherProperty = 10 };
            set.Add(obj);
            obj.OtherProperty = 100;

            Assert.IsFalse(set.Contains(obj));
        }

        /// Life is easier when the GetHashCode of an object doesn't
        /// change ever.

        class AClassWithANicerGetHashCode
        {
            public string Id { get { return id; } }
            public int OtherProperty { get; set; }

            public AClassWithANicerGetHashCode(string id, int otherProperty)
            {
                this.id = id;
                OtherProperty = otherProperty;
            }

            public override int GetHashCode()
            {
                return id.GetHashCode();
            }

            readonly private string id;
        }


        [TestMethod]
        public void NiceClass_GetHashCode_Inmutable()
        {
            var obj = new AClassWithANicerGetHashCode("A", 10);
            int hashCode1 = obj.GetHashCode();
            obj.OtherProperty = 100;
            int hashCode2 = obj.GetHashCode();

            Assert.AreEqual(hashCode1, hashCode2);
        }

        [TestMethod]
        public void NiceClass_WorksWellWithCollections()
        {
            var set = new HashSet<AClassWithANicerGetHashCode>();
            var obj = new AClassWithANicerGetHashCode("A", 10);
            set.Add(obj);
            obj.OtherProperty = 100;

            Assert.IsTrue(set.Contains(obj));
        }
    }
}
