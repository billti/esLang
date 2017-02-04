//------------------------------------------------------------------------------
// <copyright file="esPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace esLang
{
    // See "Model of a legacy language service": https://msdn.microsoft.com/en-us/library/bb166518.aspx
    // See "Syntax coloring in a legacy language service": https://msdn.microsoft.com/en-us/library/bb166778.aspx
    // See "Registering a legacy language service": https://msdn.microsoft.com/en-us/library/bb166498.aspx

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(esPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideEditorFactory(typeof(EditorFactory), 101)]
    [ProvideEditorExtension(typeof(EditorFactory), ".es", 32, NameResourceID = 101)]
    [ProvideService(typeof (EsLanguageService), ServiceName = "es language service")]
    [ProvideLanguageService(typeof(EsLanguageService), "es", 106, EnableAsyncCompletion = true, CodeSense = true, MatchBraces = true, MatchBracesAtCaret = true, EnableFormatSelection = true)]
    [ProvideLanguageExtension(typeof(EsLanguageService), ".es")]
    public sealed class esPackage : Package, IOleComponent
    {
        private EditorFactory editorFactory;
        private uint m_componentID;
        /// <summary>
        /// esPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "c45ec0ab-6b2a-4f50-9b0f-a862b64a617b";

        /// <summary>
        /// Initializes a new instance of the <see cref="esPackage"/> class.
        /// </summary>
        public esPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.editorFactory = new EditorFactory(this);
            RegisterEditorFactory(this.editorFactory);

// From here on, based on sample code from "Registering a legacy language service": https://msdn.microsoft.com/en-us/library/bb166498.aspx

            // Proffer the service.  
            IServiceContainer serviceContainer = this as IServiceContainer;
            EsLanguageService langService = new EsLanguageService();
            langService.SetSite(this);
            serviceContainer.AddService(typeof(EsLanguageService),
                                        langService,
                                        true);

// If not supporting async completion (background parsing), from here down, and implementing IOLEComponent, may be unnecessary            
            
            // Register a timer to call our language service during idle periods.  
            IOleComponentManager mgr = GetService(typeof(SOleComponentManager))
                                       as IOleComponentManager;
            if (m_componentID == 0 && mgr != null)
            {
                OLECRINFO[] crinfo = new OLECRINFO[1];
                crinfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
                crinfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime |
                                              (uint)_OLECRF.olecrfNeedPeriodicIdleTime;
                crinfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal |
                                              (uint)_OLECADVF.olecadvfRedrawOff |
                                              (uint)_OLECADVF.olecadvfWarningsOff;
                crinfo[0].uIdleTimeInterval = 1000;
                int hr = mgr.FRegisterComponent(this, crinfo, out m_componentID);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (m_componentID != 0)
            {
                IOleComponentManager mgr = GetService(typeof(SOleComponentManager))
                                           as IOleComponentManager;
                if (mgr != null)
                {
                    int hr = mgr.FRevokeComponent(m_componentID);
                }
                m_componentID = 0;
            }

            base.Dispose(disposing);
        }

        public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return 1;
        }

        public int FPreTranslateMessage(MSG[] pMsg)
        {
            return 1;
        }

        public void OnEnterState(uint uStateID, int fEnter)
        {
        }

        public void OnAppActivate(int fActive, uint dwOtherThreadID)
        {
        }

        public void OnLoseActivation()
        {
        }

        public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved)
        {
        }

        public int FDoIdle(uint grfidlef)
        {
            bool bPeriodic = (grfidlef & (uint)_OLEIDLEF.oleidlefPeriodic) != 0;
            LanguageService service = GetService(typeof(EsLanguageService))
                                      as LanguageService;
            if (service != null)
            {
                service.OnIdle(bPeriodic);
            }
            return 0;
        }

        public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return 1;
        }

        public int FQueryTerminate(int fPromptUser)
        {
            return 1;
        }

        public void Terminate()
        {
        }

        public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }
        #endregion
    }
}
