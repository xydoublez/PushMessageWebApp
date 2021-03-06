﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdminHomeController.cs" company="">
//   
// </copyright>
// <summary>
//   The admin home controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebAuth.Areas.Admin.Controllers
{
    using System.ComponentModel;
    using System.Web.Mvc;

    using WebAuth.Controllers;

    /// <summary>
    /// The admin home controller.
    /// </summary>
    public partial class AdminHomeController : BaseAdminController
    {
        // GET: SuperAdmin/Home
        #region Public Methods and Operators

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [Description("AdminHomeMainPage")]
        public virtual ActionResult Index()
        {
            return this.View();
        }

        #endregion
    }
}