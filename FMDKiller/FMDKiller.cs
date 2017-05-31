using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace FMDKiller {
	// auto generated set of attributes
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	[ProvideAutoLoad(UIContextGuids80.SolutionExists)]	// auto start when solution load
	[Guid(PackageGuidString)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	public sealed class FMDKiller : Package {
		// auto generated, replace with your own value
		public const string PackageGuidString = "ca6f249c-847b-4acb-baaf-09c31e3b6546";

		#region Import WinAPI

		[DllImport("user32.dll", EntryPoint = "SetWindowsHookEx", SetLastError = true)]
		static extern IntPtr SetWindowsHookEx(int hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId")]
		static extern uint GetCurrentThreadId();

		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetClassName", SetLastError = true)]
		static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindowTextLength")]
		static extern int GetWindowTextLength(HandleRef hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindowText")]
		static extern int GetWindowText(HandleRef hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll", EntryPoint = "CallNextHookEx")]
		static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", EntryPoint = "UnhookWindowsHookEx", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

		#pragma warning disable 0649
		struct CWPSTRUCT {
			public IntPtr wParam;
			public IntPtr lParam;
			public int msg;
			public IntPtr hwnd;
		}
		#pragma warning restore 0649

		#endregion

		delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

		private const int WH_CALLWNDPROC = 4;

		private const int SHOW = 5;

		private const byte TAB = 0x09;

		private const byte RETURN = 0x0D;

		private const string DIALOG_BOX = "#32770";

		private static IntPtr hookPtr = IntPtr.Zero;

		private static HookProc hookProc;

		public FMDKiller() { }

		protected override void Initialize() {
			base.Initialize();
			ThreadHelper.Generic.Invoke(() => {
				hookProc = HookProcImpl;
				hookPtr = SetWindowsHookEx(WH_CALLWNDPROC, hookProc, IntPtr.Zero, GetCurrentThreadId());
			});
		}

		private IntPtr HookProcImpl(int code, IntPtr wParam, IntPtr lParam) {
			try {
				if (code >= 0) {
					CWPSTRUCT w = (CWPSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPSTRUCT));
					if (w.msg == 1) {
						StringBuilder stringBuilder = new StringBuilder(128);
						int num = GetClassName(w.hwnd, stringBuilder, stringBuilder.Capacity);
						if (num != 0 && stringBuilder.ToString() == DIALOG_BOX) {
							int capacity = GetWindowTextLength(new HandleRef(this, w.hwnd)) * 2;
							StringBuilder stringBuilder2 = new StringBuilder(capacity);
							GetWindowText(new HandleRef(this, w.hwnd), stringBuilder2, stringBuilder2.Capacity);
							string caption = stringBuilder2.ToString();
							if (caption == "File Modification Detected" ||
							    caption == "Обнаружено изменение в файле") {
								ShowWindow(w.hwnd, SHOW);
								keybd_event(TAB, 0, 0, 0);
								keybd_event(RETURN, 0, 0, 0);
							}
						}
					}
				}
			} catch (Exception e) {
				Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Exception: {0}", e.Message), "FMDKiller");
			}
			GC.KeepAlive(this);
			return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
		}

		protected override void Dispose(bool disposing) {
			if (hookPtr != IntPtr.Zero) {
				UnhookWindowsHookEx(hookPtr);
			}
			base.Dispose(disposing);
		}
	}
}
