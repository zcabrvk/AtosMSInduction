using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtosInduction
{
    /// <summary>
    /// Interface to decouple application from implementation of queries to database.
    /// </summary>
    public interface Database
    {
        /// <summary>
        /// If the User details have been stored in a file(to allow automatic login), it deletes them.
        /// </summary>
        /// <returns>Task</returns>
        Task deleteLoginDetails();

        /// <summary>
        /// Checks whether the User details have been stored in order
        /// to allow automatic login.
        /// </summary>
        /// <returns>True if the urer credentials are stored, false otherwise.</returns>
        Task<bool> areUserDetailsLogged();

        /// <summary>
        /// Try to login user automatically using details stored from previous access.
        /// </summary>
        /// <exception crf="Exception">if there is no file stored or if the login from stored details fails</exception>>
        /// <returns></returns>
        Task loginFromStoredDetails();

        /// <summary>
        /// Perform login process taking the user to apposite page where it gets username and password,
        /// queries database and process answer. It returns to starting page when done.
        /// </summary>
        /// <exception>if the login process fails</exception>>
        /// <returns></returns>
        Task performLoginProcess();

        /// <summary>
        /// Log the user out.
        /// </summary>
        /// <returns></returns>
        Task forceLogout();

        /// <summary>
        /// Queries database to get User's full name.
        /// </summary>
        /// <returns>full User's name or empty string if query failed</returns>
        Task<string> getUserFullName();

        /// <summary>
        /// Stores the Login details of the user in a file.
        /// </summary>
        /// <exception>if the user is not logged in</exception>>
        /// <returns></returns>
        Task storeLoginDetails();
    }
}
