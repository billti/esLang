﻿using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System;

namespace esLang
{
    // TODO: Should probably replace with Microsoft.VisualStudio.Package.EditorFactor derived class
    // Code largely from sample at https://msdn.microsoft.com/en-us/library/bb165071.aspx

    [Guid("1096A617-CFD7-4488-9C75-DA2FDEF5A0C8")]
    class EditorFactory : IVsEditorFactory
    {
        private Package parentPackage;
        private IOleServiceProvider serviceProvider;

        public EditorFactory(Package parentPackage)
        {
            this.parentPackage = parentPackage;
        }

        public int CreateEditorInstance(
                uint grfCreateDoc, 
                string pszMkDocument, 
                string pszPhysicalView, 
                IVsHierarchy pvHier, 
                uint itemid, 
                IntPtr punkDocDataExisting, 
                out IntPtr ppunkDocView, 
                out IntPtr ppunkDocData, 
                out string pbstrEditorCaption, 
                out Guid pguidCmdUI, 
                out int pgrfCDW)
        {
            int retval = VSConstants.E_FAIL;

            // Initialize these to empty to start with  
            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;
            pbstrEditorCaption = "";
            pguidCmdUI = Guid.Empty;
            pgrfCDW = 0;

            if ((grfCreateDoc & (VSConstants.CEF_OPENFILE |
                  VSConstants.CEF_SILENT)) == 0)
            {
                throw new ArgumentException("Only Open or Silent is valid");
            }
            if (punkDocDataExisting != IntPtr.Zero)
            {
                return VSConstants.VS_E_INCOMPATIBLEDOCDATA;
            }

            // Instantiate a text buffer of type VsTextBuffer.  
            // Note: we only need an IUnknown (object) interface for   
            // this invocation.  
            Guid clsidTextBuffer = typeof(VsTextBufferClass).GUID;
            Guid iidTextBuffer = VSConstants.IID_IUnknown;
            object pTextBuffer = pTextBuffer = parentPackage.CreateInstance(
                  ref clsidTextBuffer,
                  ref iidTextBuffer,
                  typeof(object));

            if (pTextBuffer != null)
            {
                // "Site" the text buffer with the service provider we were  
                // provided.  
                IObjectWithSite textBufferSite = pTextBuffer as IObjectWithSite;
                if (textBufferSite != null)
                {
                    textBufferSite.SetSite(this.serviceProvider);
                }

                // Instantiate a code window of type IVsCodeWindow.  
                Guid clsidCodeWindow = typeof(VsCodeWindowClass).GUID;
                Guid iidCodeWindow = typeof(IVsCodeWindow).GUID;
                IVsCodeWindow pCodeWindow =
                (IVsCodeWindow)this.parentPackage.CreateInstance(
                      ref clsidCodeWindow,
                      ref iidCodeWindow,
                      typeof(IVsCodeWindow));
                if (pCodeWindow != null)
                {
                    // Give the text buffer to the code window.  
                    // We are giving up ownership of the text buffer!  
                    pCodeWindow.SetBuffer((IVsTextLines)pTextBuffer);

                    // Now tell the caller about all this new stuff   
                    // that has been created.  
                    ppunkDocView = Marshal.GetIUnknownForObject(pCodeWindow);
                    ppunkDocData = Marshal.GetIUnknownForObject(pTextBuffer);

                    // Specify the command UI to use so keypresses are   
                    // automatically dealt with.  
                    pguidCmdUI = VSConstants.GUID_TextEditorFactory;

                    // This caption is appended to the filename and  
                    // lets us know our invocation of the core editor   
                    // is up and running.  
                    pbstrEditorCaption = " [es]";

                    retval = VSConstants.S_OK;
                }
            }
            return retval;
        }

        public int SetSite(IOleServiceProvider psp)
        {
            this.serviceProvider = psp;
            return VSConstants.S_OK;
        }

        public int Close()
        {
            return VSConstants.S_OK;
        }

        public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            int retval = VSConstants.E_NOTIMPL;
            pbstrPhysicalView = null;
            if(rguidLogicalView.Equals(VSConstants.LOGVIEWID_Designer)
                || rguidLogicalView.Equals(VSConstants.LOGVIEWID_Primary))
            {
                retval = VSConstants.S_OK;
            }
            return retval;
        }
    }
}
