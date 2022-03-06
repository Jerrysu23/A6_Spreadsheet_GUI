// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System;
using System.Collections.Generic;

/// <summary>
/// <author>
/// Wenlin Li
/// </author>
/// <remarks>
/// Written on September 10, 2021.
/// </remarks>
/// </summary>

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {   /// <summary>
        /// A hash map contains nodes that are depended upon
        /// </summary>
        private Dictionary<string, HashSet<string>> dependeesMap;
        /// <summary>
        /// A hash map contains nodes that depends on another nodes
        /// </summary>
        private Dictionary<string, HashSet<string>> dependentsMap;


        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            Size = 0;
            dependeesMap = new Dictionary<string, HashSet<string>>();
            dependentsMap = new Dictionary<string, HashSet<string>>();
        }

        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get;
            private set;
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                if (dependeesMap.TryGetValue(s, out HashSet<string> dependees))
                    return dependees.Count;

                return 0;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("Nothing need to look for");
            }
            // If the dependents of s is in the dependentsMap and it is not null
            // and there is at least one HashSet inside, then return true.
            dependentsMap.TryGetValue(s, out HashSet<string> dependents);
            if (dependents != null && dependents.Count > 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("Nothing need to look for");
            }
            // If the dependees of s is in the dependeesMap and it is not null
            // and there is at least one HashSet inside, then return true.
            dependeesMap.TryGetValue(s, out HashSet<string> dependees);
            if (dependees != null && dependees.Count > 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            HashSet<string> dependents;
            if (s == null)
            {
                throw new ArgumentNullException("Null can not be allowed");
            }
            // If s already has dependents, return those dependents.
            if (dependentsMap.TryGetValue(s, out dependents))
            {
                return new HashSet<string>(dependents);
            }
            // Otherwise return an empty HashSet.
            return new HashSet<string>();

        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            HashSet<string> dependees;
            if (s == null)
            {
                throw new ArgumentNullException("Null can not be allowed");
            }
            // If s already has dependents, return those dependents.
            if (dependeesMap.TryGetValue(s, out dependees))
            {
                return new HashSet<string>(dependees);
            }
            // Otherwise return an empty HashSet.
            return new HashSet<string>();
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            if (s == null || t == null)
            {
                throw new ArgumentNullException("Nothing need to look for");
            }

            HashSet<string> dependees;
            HashSet<string> dependents;

            // If the dependents don't have the value, then create a new HashSet named dependents. 
            if (!dependentsMap.TryGetValue(s, out dependents))
            {
                dependents = new HashSet<string>();
                // Add s and dependents into dependentsMap.
                dependentsMap.Add(s, dependents);
                // Add t into dependents.
                dependents.Add(t);
            }
            else
            {
                // If t is in dependents HashSet, add t into the dependents HashSet.
                if (!dependents.Contains(t))
                {
                    dependents.Add(t);
                }
                else
                {
                    // If the dependents already have the s string just return.
                    return;
                }
            }
            // If the dependees don't have the value, then create a new HashSet named dependees.
            if (!dependeesMap.TryGetValue(t, out dependees))
            {
                dependees = new HashSet<string>();
                // Add the t and dependees into dependeesMap.
                dependeesMap.Add(t, dependees);
                // Add s into the dependees.
                dependees.Add(s);
            }
            dependees.Add(s);

            Size++;
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (s == null || t == null)
            {
                throw new ArgumentNullException("Nothing need to remove");
            }
            HashSet<string> dependees;
            HashSet<string> dependents;

            // If the dependents already have the dependents that we are looking for. 
            if (dependentsMap.TryGetValue(s, out dependents))
            {   // Determine if the string t is in the dependents. 
                if (dependents.Contains(t))
                {
                    // If dependents contains the string t, remove it.
                    dependents.Remove(t);
                }
            }
            // If the dependees already have the dependees that we are looking for. 
            if (dependeesMap.TryGetValue(t, out dependees))
            {   // Determine if the string t is in the dependees.
                if (dependees.Contains(s))
                {
                    // If dependees contains the string s, remove it.
                    dependees.Remove(s);
                }
            }

            Size--;
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (s == null || newDependents == null)
            {
                throw new ArgumentNullException("Nothing need to look for or nothing to replace");
            }
            // If we can get the dependents from the key s, just simply clear it.
            HashSet<string> dependents;
            if (dependentsMap.TryGetValue(s, out dependents) && dependents != null)
            {
                IEnumerable<string> dependentsRemove = GetDependents(s);
                foreach (string remove in dependentsRemove)
                {
                    RemoveDependency(s, remove);
                }

            }
            // If not, make a new HashSet for key s into the dependentsMap.
            else
            {
                dependentsMap.Add(s, new HashSet<string>());
            }
            // Add the new dependents element by the AddDependency method.
            foreach (string newDp in newDependents)
            {
                AddDependency(s, newDp);
            }


        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if (s == null || newDependees == null)
            {
                throw new ArgumentNullException("Nothing need to look for and nothing to replace");
            }
            // If we can get the dependees from the key s, just simply clear it.
            HashSet<string> dependees;
            if (dependeesMap.TryGetValue(s, out dependees) && dependees != null)
            {
                IEnumerable<string> dependeesRemove = GetDependees(s);
                foreach (string remove in dependeesRemove)
                {
                    RemoveDependency(remove, s);
                }
            }
            // If not, make a new HashSet for key s into the dependeesMap.
            else
            {
                dependeesMap.Add(s, new HashSet<string>());
            }
            // Add the new dependees element by the AddDependency method.
            foreach (string newDp in newDependees)
            {
                AddDependency(newDp, s);
            }

        }

    }

}