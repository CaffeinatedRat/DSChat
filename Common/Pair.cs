using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    class Pair<F,S>
    {
        #region Properties

        /// Last Modified: 12/5/09
        /// <summary>
        /// Modify or return the first element in the pair.
        /// </summary>
        public F First { get; set; }

        /// Second
        /// Last Modified: 12/5/09
        /// <summary>
        /// Modify or return the second element in the pair.
        /// </summary>
        public S Second { get; set; }

        #endregion Properties

        #region Methods

        /// Last Modified: 12/5/09
        /// <summary>
        /// Constructs a pair object.
        /// </summary>
        /// -----------------------------------------------------
        /// PRECONDITIONS: NA -- No preconditions exist.
        /// -----------------------------------------------------
        /// Parameters:
        /// <param name="f">
        /// The first element in the pair.
        /// </param>
        /// <param name="s">
        /// The second element in the pair.
        /// </param>
        /// -----------------------------------------------------
        /// POSTCONDITIONS: Refer to the return statement.
        /// -----------------------------------------------------
        /// Return Value:
        public Pair(F f, S s)
        {
            First = f;
            Second = s;
        }

        #endregion Methods

    }   //END OF CLASS
}   //END OF NAMESPACE
