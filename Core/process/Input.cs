using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Input;


namespace Core.process
{
    public static class Input
    {
        private static IKeyboard _keyboard;
        private static IMouse _mouse;

        private static readonly bool[] _keys = new bool[512];
        private static readonly bool[] _mouseButtons = new bool[5];

        public static event Action<Key>? KeyPressed;
        public static event Action<Key>? KeyReleased;

        public static event Action<MouseButton>? MouseClicked;
        public static event Action<Vector2>? MouseMove;
        public static event Action<MouseButton>? MouseReleased;

        public static Vector2 MousePosition { get; private set; }
        public static Vector2 ScrollDelta { get; private set; }

        public static bool isActive = false;

        public static void Init(IInputContext input)
        {
            if (input.Keyboards.Count > 0)
            {
                _keyboard = input.Keyboards[0];
                _keyboard.KeyDown += (_, key, _) =>
                {
                    _keys[(int)key] = true;
                    KeyPressed?.Invoke(key);
                };
                _keyboard.KeyUp += (_, key, _) =>
                {
                    _keys[(int)key] = false;
                    KeyReleased?.Invoke(key);
                };
            }

            if (input.Mice.Count > 0)
            {
                _mouse = input.Mice[0];
                _mouse.MouseDown += (_, btn) =>
                {
                    _mouseButtons[(int)btn] = true;
                    MouseClicked?.Invoke(btn);
                };
                _mouse.MouseUp += (_, btn) =>
                {
                    _mouseButtons[(int)btn] = false;
                    MouseReleased?.Invoke(btn);
                };
                _mouse.MouseMove += (_, pos) =>
                {
                    MouseMove?.Invoke(pos);
                    MousePosition = pos;
                };
                _mouse.Scroll += (_, scroll) => ScrollDelta = new Vector2(scroll.X, scroll.Y);
            }
        }

        public static bool KeyFunc(Key key) => _keys[(int)key];
        public static bool Mouse(MouseButton btn) => _mouseButtons[(int)btn];
        public static void ResetScroll() => ScrollDelta = Vector2.Zero;

        // Shortcut-uri pentru fiecare tastă
        public static bool KeyA => KeyFunc(Key.A);
        public static bool KeyB => KeyFunc(Key.B);
        public static bool KeyC => KeyFunc(Key.C);
        public static bool KeyD => KeyFunc(Key.D);
        public static bool KeyE => KeyFunc(Key.E);
        public static bool KeyF => KeyFunc(Key.F);
        public static bool KeyG => KeyFunc(Key.G);
        public static bool KeyH => KeyFunc(Key.H);
        public static bool KeyI => KeyFunc(Key.I);
        public static bool KeyJ => KeyFunc(Key.J);
        public static bool KeyK => KeyFunc(Key.K);
        public static bool KeyL => KeyFunc(Key.L);
        public static bool KeyM => KeyFunc(Key.M);
        public static bool KeyN => KeyFunc(Key.N);
        public static bool KeyO => KeyFunc(Key.O);
        public static bool KeyP => KeyFunc(Key.P);
        public static bool KeyQ => KeyFunc(Key.Q);
        public static bool KeyR => KeyFunc(Key.R);
        public static bool KeyS => KeyFunc(Key.S);
        public static bool KeyT => KeyFunc(Key.T);
        public static bool KeyU => KeyFunc(Key.U);
        public static bool KeyV => KeyFunc(Key.V);
        public static bool KeyW => KeyFunc(Key.W);
        public static bool KeyX => KeyFunc(Key.X);
        public static bool KeyY => KeyFunc(Key.Y);
        public static bool KeyZ => KeyFunc(Key.Z);

        public static bool Key0 => KeyFunc(Key.Number0);
        public static bool Key1 => KeyFunc(Key.Number1);
        public static bool Key2 => KeyFunc(Key.Number2);
        public static bool Key3 => KeyFunc(Key.Number3);
        public static bool Key4 => KeyFunc(Key.Number4);
        public static bool Key5 => KeyFunc(Key.Number5);
        public static bool Key6 => KeyFunc(Key.Number6);
        public static bool Key7 => KeyFunc(Key.Number7);
        public static bool Key8 => KeyFunc(Key.Number8);
        public static bool Key9 => KeyFunc(Key.Number9);

        public static bool KeyEscape => KeyFunc(Key.Escape);
        public static bool KeySpace => KeyFunc(Key.Space);
        public static bool KeyEnter => KeyFunc(Key.Enter);
        public static bool KeyTab => KeyFunc(Key.Tab);
        public static bool KeyBackspace => KeyFunc(Key.Backspace);
        public static bool KeyLeftShift => KeyFunc(Key.ShiftLeft);
        public static bool KeyRightShift => KeyFunc(Key.ShiftRight);
        public static bool KeyLeftCtrl => KeyFunc(Key.ControlLeft);
        public static bool KeyRightCtrl => KeyFunc(Key.ControlRight);
        public static bool KeyLeftAlt => KeyFunc(Key.AltLeft);
        public static bool KeyRightAlt => KeyFunc(Key.AltRight);
        public static bool KeyUp => KeyFunc(Key.Up);
        public static bool KeyDown => KeyFunc(Key.Down);
        public static bool KeyLeft => KeyFunc(Key.Left);
        public static bool KeyRight => KeyFunc(Key.Right);
    }
}
