using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using gma.System.Windows;
using System.Threading;
using Shell32;
using System.Timers;
using System.Runtime.InteropServices;


namespace WindowsCraft2._1
{

    public partial class Form1 : Form
    {
        #region DATA
        private static System.Timers.Timer aTimer;
        public enum ModifierFlags : uint
        {
            None = 0,
            Alt = 0,
            Control = 0,
            Shift = 0,
        }

      public static class GlobalVars
      {
        public static int Fpathtxt_X; // stores the x and y of the last textboxes to be created so we can calculate where to position the next row of boxes
        public static int Fpathtxt_Y;
        public static int Bindtxt_X;
        public static int Bindtxt_Y;
        public static int currFormNum;
        public static string formname = "BINDS";
        public static string formname1 = "FILEPATH";
        public static const int MaxBinds = 15;
        public static const int FormYBounds = 403 - 20;
        public static bool ConcatenateMode = false; // add shortcuts to existing units
        public static Byte[] keys = new Byte[256];
        public static ArrayList selected = new ArrayList();
        public static KeyEventArgs PrevKeyState;
        public static float timeSincePressed;
        public static KeyEventArgs CurrKeyState;
        public static float doubleTapTime = 0.5f; // half a second
       //  public static Byte[] blacklist = new Byte[] {  } black list certain keys

      }
        #endregion
      #region .imports

      [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
      public static extern IntPtr GetForegroundWindow();

      [DllImport("kernel32.dll")]

      public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,

          uint dwSize, uint flAllocationType, uint flProtect);

      [DllImport("kernel32.dll")]

      public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,

         uint dwSize, uint dwFreeType);

      [DllImport("kernel32.dll")]

      public static extern bool CloseHandle(IntPtr handle);

      [DllImport("kernel32.dll")]

      public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,

         IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);

      [DllImport("kernel32.dll")]

      public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,

         IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);

      [DllImport("kernel32.dll")]

      public static extern IntPtr OpenProcess(uint dwDesiredAccess,

          bool bInheritHandle, uint dwProcessId);

      [DllImport("user32.DLL")]

      public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

      [DllImport("user32.DLL")]

      public static extern IntPtr FindWindow(string lpszClass, string lpszWindow);

      [DllImport("user32.DLL")]

      public static extern IntPtr FindWindowEx(IntPtr hwndParent,

          IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

      [DllImport("user32.dll")]

      public static extern uint GetWindowThreadProcessId(IntPtr hWnd,

          out uint dwProcessId);
      #endregion

        /*
      #region BindListener Object
     
      class BindListener
      {
          bool doublePressed;
          ShellItems cmdshell = new ShellItems();
          List<BindHash> Binds = new List<BindHash>();
          public KeyEventArgs receivekeystate;
          public void BindListener(KeyEventArgs state)
          {
              
              int num = 0;
       
          }

          public bool IsBindPress() // decipher wheather they are binding a key or wheather they want to launch the actual action
          {

              return false;
          }
        public bool IsExecuteOrder()
        {
            if (timeSincePressed <= doubleTapTime && GlobalVars.PrevKeyState == GlobalVars.CurrKeyState)
            {
                doublePressed = true;
                if(BindExists())
               {
                cmdshell.ShellEx();
               }
            }
            doublePressed = false;
            return doublePressed;
         }
          public int ReceiveKeys() 
          {
              if (BindExists()) // the keys pressed meet the standards and don't overlap with other binds if they overlap rewrite or bind concatenate feature turned on
             {
                 return -1;
             }   
         IsBindPress(); // if a desktop window or shell explorer window is in the foreground and items are selected
         IsExecuteOrder(); // if time between presses is small and both presses are the same indicating a double tap
         cmdshell.GetListOfSelectedFiles();  

              return 1;
          }
          public bool BindExists(bool addbind) // are we checking if it exists for adding or for program starting
          {
              int numNodes = 0;
              numNodes = Binds.Count;
              for (int c = 0; c < numNodes; c++)
              {
                  if (binds[c].keystate = receivekeystate)
                  {
                      MessageBox.Show("Bind exists");
                      return true;
                     if(GlobalVars.ConcatenateMode == true)
                     {
                         NewBindInstance();
          
         
                       }
                      return true;
                  }
                  return false;
              }
              return false;
          }
          public int NewBindInstance()
          {
              BindHash CreateBind = new BindHash();
               
          }
      }
       
      
      #endregion
      */

      /*
    #region BindTable data structure
    public class BindHash
      {
          int formID;
          public int[] keys;
          public KeyEventArgs keystate;
          public string[] Filepaths;


          public BindHash(int[] binds,string[] paths,int numpaths,int numkeys)
          {
              formID = GlobalVars.currFormNum;
              for (int j = 0; j < numkeys; j++)
              {
                  keys[j] = binds[j];
              }
              Filepaths = new string[numpaths];
              for (int k = 0; k < numpaths; k++)
              {
                  Filepaths[k] = paths[k];
              }

          }
          public int[] GetKeybinds()
          {
              return keys;
          }
          public string[] GetAssociatedFiles()
          {
              return Filepaths;
          }
          
      }
    #endregion
       * */
        
        /*
        #region ShellIO .text
        public class ShellItems
        {
            IntPtr vHandle;
            int vItemCount;
            IntPtr vPointer;
            IntPtr vProcess;

            [StructLayoutAttribute(LayoutKind.Sequential)]
            private struct LVITEM
            {
                public uint mask;
                public int iItem;
                public int iSubItem;
                public uint state;
                public uint stateMask;
                public IntPtr pszText;
                public int cchTextMax;
                public int iImage;
                public IntPtr lParam;
            }

            const int LVM_FIRST = 0x1000;
            const int LVM_GETSELECTEDCOUNT = 4146;
            const int LVM_GETNEXTITEM = LVM_FIRST + 12;
            const int LVNI_SELECTED = 2;
            const int LVM_GETITEMCOUNT = LVM_FIRST + 4;
            const int LVM_GETITEM = LVM_FIRST + 75;
            const int LVIF_TEXT = 0x0001;



            public const uint LVM_GETITEMW = LVM_FIRST + 75;
            public const uint LVM_GETITEMPOSITION = LVM_FIRST + 16;
            public const uint PROCESS_VM_OPERATION = 0x0008;
            public const uint PROCESS_VM_READ = 0x0010;
            public const uint PROCESS_VM_WRITE = 0x0020;
            public const uint MEM_COMMIT = 0x1000;
            public const uint MEM_RELEASE = 0x8000;
            public const uint MEM_RESERVE = 0x2000;
            public const uint PAGE_READWRITE = 4;

            public int GetShellWindow()
            {
                vHandle = FindWindow("Progman", "Program Manager");

                vHandle = FindWindowEx(vHandle, IntPtr.Zero, "SHELLDLL_DefView", null);

                vHandle = FindWindowEx(vHandle, IntPtr.Zero, "SysListView32", "FolderView");

                vItemCount = SendMessage(vHandle, LVM_GETITEMCOUNT, 0, 0);
                return vItemCount;

            }
           public int OpenProc()
           {
               uint vProcessId;

               GetWindowThreadProcessId(vHandle, out vProcessId);

 
                vProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ |

                PROCESS_VM_WRITE, false, vProcessId);

               vPointer = VirtualAllocEx(vProcess, IntPtr.Zero, 4096,

                   MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
               return 0;


           }
            
            public string GetItemText(int idx)
            {
                // Declare and populate the LVITEM structure
                LVITEM lvi = new LVITEM();
                lvi.mask = LVIF_TEXT;
                lvi.cchTextMax = 512;
                lvi.iItem = idx;            // the zero-based index of the ListView item
                lvi.iSubItem = 0;         // the one-based index of the subitem, or 0 if this
                //  structure refers to an item rather than a subitem
                lvi.pszText = Marshal.AllocHGlobal(512);

                // Send the LVM_GETITEM message to fill the LVITEM structure
                IntPtr ptrLvi = Marshal.AllocHGlobal(Marshal.SizeOf(lvi));
                Marshal.StructureToPtr(lvi, ptrLvi, false);
                try
                {
                    SendMessage(ShellListViewHandle, LVM_GETITEM, IntPtr.Zero, ptrLvi);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                // Extract the text of the specified item
                string itemText = Marshal.PtrToStringAuto(lvi.pszText);
                return itemText;
            }
             
            public Array GetItemsTexts()
            {
                try
                {
                    for (int j = 0; j < vItemCount; j++)
                    {

                        byte[] vBuffer = new byte[256];

                        LVITEM[] vItem = new LVITEM[1];

                        vItem[0].mask = LVIF_TEXT;

                        vItem[0].iItem = j;

                        vItem[0].iSubItem = 0;

                        vItem[0].cchTextMax = vBuffer.Length;

                        vItem[0].pszText = (IntPtr)((int)vPointer + Marshal.SizeOf(typeof(LVITEM)));

                        uint vNumberOfBytesRead = 0;



                        WriteProcessMemory(vProcess, vPointer,

                            Marshal.UnsafeAddrOfPinnedArrayElement(vItem, 0),

                            Marshal.SizeOf(typeof(LVITEM)), ref vNumberOfBytesRead);

                        SendMessage(vHandle, LVM_GETITEMW, j, vPointer.ToInt32());

                        ReadProcessMemory(vProcess,

                            (IntPtr)((int)vPointer + Marshal.SizeOf(typeof(LVITEM))),

                            Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0),

                            vBuffer.Length, ref vNumberOfBytesRead);

                        string vText = Encoding.Unicode.GetString(vBuffer, 0,

                            (int)vNumberOfBytesRead);

                        string IconName = vText;

                        //Get icon location

                        SendMessage(vHandle, LVM_GETITEMPOSITION, j, vPointer.ToInt32());

                        Point[] vPoint = new Point[1];

                        ReadProcessMemory(vProcess, vPointer,

                            Marshal.UnsafeAddrOfPinnedArrayElement(vPoint, 0),

                            Marshal.SizeOf(typeof(Point)), ref vNumberOfBytesRead);

                        string IconLocation = vPoint[0].ToString();
                    }

                }

                finally
                {
                    VirtualFreeEx(vProcess, vPointer, 0, MEM_RELEASE);

                    CloseHandle(vProcess);
                }
            }

            IntPtr ShellListViewHandle
            {
                get
                {
                    IntPtr _ProgMan = GetShellWindow();
                    IntPtr _SHELLDLL_DefViewParent = _ProgMan;
                    IntPtr _SHELLDLL_DefView = FindWindowEx(_ProgMan, IntPtr.Zero, "SHELLDLL_DefView", null);
                    IntPtr _SysListView32 = FindWindowEx(_SHELLDLL_DefView, IntPtr.Zero, "SysListView32", "FolderView");
                    return _SysListView32;
                }
            }

            public int GetSelectedItemIndex(int iPos = -1)
            {
                return SendMessage(ShellListViewHandle, LVM_GETNEXTITEM, iPos, LVNI_SELECTED);
            }
            public unsafe int ShellEx(string[] link, int opt)
            {
                //if ((int)ShellExecuteW(NULL, L"open", link, NULL, NULL, SW_SHOWNORMAL) < 32)
                Process p = new Process();

                System.Diagnostics.ProcessStartInfo pi = new ProcessStartInfo();

                pi.UseShellExecute = true;

                pi.FileName = link[opt]->ToString();

                p.StartInfo = pi;

                p.Start();

                if (opt-- > 0)
                {
                    ShellEx(link, opt);
                }

                return 0;
            }
        }
        // ~~~~~~~~~ END SHELLIO SECTION ~~~~~~~~~ \\
        #endregion
         * */
         


        // ~~~~~~~~~ START CONTROLS SECTION ~~~~~~~~~ \\
        #region ControlsIO .text

        public Form1()
        {
            InitializeComponent();
            UserActivityHook KBDHOOK = new UserActivityHook();
            KBDHOOK.KeyDown += new KeyEventHandler(MyKeyDown);
            KBDHOOK.KeyPress += new KeyPressEventHandler(MyKeyPress);
            KBDHOOK.KeyUp += new KeyEventHandler(MyKeyUp);
             

           // BindListener Listen = new BindListener();
          //  Thread ListenerThread = new Thread(new ThreadStart(Listen.BindListener));
           // Thread KBDHOOKTHREAD = new Thread(new ThreadStart());
           // ListenerThread.Start();
        }
        private void MyKeyDown(object sender, KeyEventArgs e)
        {
            GlobalVars.timeSincePressed = 0;
            GlobalVars.CurrKeyState = e;
            if (e.Modifiers == 0) // no modifiers
            {


            }
           // CurrHighlight.AppendText(string.Format("KeyDown - {0}\n", e.KeyCode));
          //  CurrHighlight.ScrollToCaret();
        }

        private void MyKeyUp(object sender, KeyEventArgs e)
        {
            GlobalVars.timeSincePressed = elapsedtime;
            GlobalVars.PrevKeyState = e;
            CurrHighlight.AppendText(string.Format("KeyUp - {0}\n", e.KeyCode));
            CurrHighlight.ScrollToCaret();
        }


        private void MyKeyPress(object sender, KeyPressEventArgs e)
        {
            CurrHighlight.AppendText(string.Format("KeyPress - {0}\n", e.KeyChar));
            CurrHighlight.ScrollToCaret();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) { GlobalVars.ConcatenateMode = true; }
            else { GlobalVars.ConcatenateMode = false; }
        }

        public void CreateBoxes()
            {
                TextBox txt = new TextBox();
                txt.Size = new Size(116,20);
                txt.Location = new Point(20, GlobalVars.Bindtxt_Y+35);
                GlobalVars.currFormNum++;
                txt.Name = GlobalVars.formname += GlobalVars.currFormNum.ToString();

                TextBox fpath1 = new TextBox();
                fpath1.Size = new Size(263,20);
                txt.Location = new Point(179,GlobalVars.Fpathtxt_Y+35);
                fpath1.Name = GlobalVars.formname1 += GlobalVars.currFormNum.ToString();

                this.Controls.Add(txt);
                this.Controls.Add(fpath1);

            }
        public void WriteToForm(int[] keydata,int numkeys)
        {
            TextBox Cntrlfind = this.Controls.Find(GlobalVars.formname += GlobalVars.currFormNum.ToString(), true).FirstOrDefault() as TextBox;
            TextBox Cntrlfind1 = this.Controls.Find(GlobalVars.formname1 += GlobalVars.currFormNum.ToString(), true).FirstOrDefault() as TextBox;


            Cntrlfind.Text = keydata[0].ToString();
            if (numkeys > 1)
            {
                Cntrlfind.AppendText("+");
                Cntrlfind.AppendText(keydata[1].ToString());
            }
            if (numkeys > 2)
            {
                Cntrlfind.AppendText("+");
                Cntrlfind.AppendText(keydata[2].ToString());
            }

            this.CreateBoxes();

        }
        public void DeleteBox()
        {
            TextBox Cntrlfind = this.Controls.Find(GlobalVars.formname += GlobalVars.currFormNum.ToString(), true).FirstOrDefault() as TextBox;
            TextBox Cntrlfind1 = this.Controls.Find(GlobalVars.formname1 += GlobalVars.currFormNum.ToString(), true).FirstOrDefault() as TextBox;

            this.Controls.Remove(Cntrlfind);
            Cntrlfind.Dispose();
            this.Controls.Remove(Cntrlfind1);
            Cntrlfind1.Dispose();

            GlobalVars.Fpathtxt_Y -= 35;
            GlobalVars.Bindtxt_Y -= 35;
            GlobalVars.currFormNum--;
        }
        #endregion
        // ~~~~~~~~~ END CONTROLS SECTION ~~~~~~~~~ \\
    }
}