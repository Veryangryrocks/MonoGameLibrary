using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Input;

public static class InputManager
{
    public enum MouseButtons
    {
        LEFT,
        MIDDLE,
        RIGHT
    }
    private static KeyboardState _keyboardState;
    private static MouseState _mouseState;

    private static Dictionary<string, InputWrapper> _inputWrappersDict;

    public static int CursorX { get; private set; }
    public static int CursorY { get; private set; }
    
    public static void Initialize()
    {
        _inputWrappersDict = new Dictionary<string, InputWrapper>();
    }

    public static void Add(string key, InputWrapper inputWrapper)
    {
        if (inputWrapper == null || _inputWrappersDict.ContainsKey(key))
        {
            return;
        }
        _inputWrappersDict.Add(key, inputWrapper);
    }

    public static bool Get(string key)
    {
        if (!_inputWrappersDict.ContainsKey(key))
        {
            return false;
        }
        return _inputWrappersDict[key].IsActive();
    }

    public static void Update()
    {
        _keyboardState = Keyboard.GetState();
        _mouseState = Mouse.GetState();

        Point cursorPos = WindowManager.ScreenToGame(_mouseState.Position);
        CursorX = cursorPos.X;
        CursorY = cursorPos.Y;

        foreach (KeyValuePair<string, InputWrapper> kvp in _inputWrappersDict)
        {
            kvp.Value.Update();
        }
    }

    public abstract class Input
    {
        protected bool _isInversed;
        public Input(bool isInversed = false)
        {
            _isInversed = isInversed;
        }
        public abstract bool IsActive();
    }

    public sealed class Key : Input
    {
        private Keys _key;
        public Key(Keys key, bool isInversed = false) : base(isInversed)
        {
            _key = key;
        }

        public override bool IsActive() => _keyboardState.IsKeyDown(_key) ^ _isInversed;
    }

    public sealed class MouseButton : Input
    {
        private MouseButtons _mouseButton;
        public MouseButton(MouseButtons mouseButton, bool isInversed = false) : base(isInversed)
        {
            _mouseButton = mouseButton;
        }
        public override bool IsActive()
        {
            switch (_mouseButton)
            {
                case MouseButtons.LEFT:
                    return _mouseState.LeftButton == ButtonState.Pressed ^ _isInversed;
                case MouseButtons.MIDDLE:
                    return _mouseState.MiddleButton == ButtonState.Pressed ^ _isInversed;
                case MouseButtons.RIGHT:
                    return _mouseState.RightButton == ButtonState.Pressed ^ _isInversed;
                default:
                    return false;
            }
        }
    }
    
    public abstract class InputBind
    {
        protected Input[] _inputsArray;
        public int Length => _inputsArray.Length;
        public InputBind(Input[] inputsArray)
        {
            _inputsArray = inputsArray;
        }
        public abstract bool IsActive();
    }

    public sealed class AndInputBind : InputBind
    {
        public AndInputBind(Input[] inputsArray) : base(inputsArray) {}
        public override bool IsActive()
        {
            foreach (Input input in _inputsArray)
            {
                if (!input.IsActive())
                {
                    return false;
                }
            }
            return true;
        }
    }

    public sealed class OrInputBind : InputBind
    {
        public OrInputBind(Input[] inputsArray) : base(inputsArray) {}
        public override bool IsActive()
        {
            foreach (Input input in _inputsArray)
            {
                if (input.IsActive())
                {
                    return true;
                }
            }
            return false;
        }
    }

    public abstract class InputWrapper
    {
        protected InputBind _inputBind;
        public InputWrapper(InputBind inputBind)
        {
            _inputBind = inputBind;
        }
        public abstract void Update();
        public abstract bool IsActive();
    }

    public sealed class Hold : InputWrapper
    {
        private int _framesHeld;
        public Hold(InputBind inputBind) : base(inputBind)
        {
            _framesHeld = 0;
        }
        public override void Update()
        {
            if (_inputBind.IsActive())
            {
                _framesHeld++;
            }
            else
            {
                _framesHeld = 0;
            }
        }

        public override bool IsActive() => _framesHeld >= 1;
    }

    public sealed class Tap : InputWrapper
    {
        private int _framesHeld;
        public Tap(InputBind inputBind) : base(inputBind)
        {
            _framesHeld = 0;
        }
        public override void Update()
        {
            if (_inputBind.IsActive())
            {
                _framesHeld++;
            }
            else
            {
                _framesHeld = 0;
            }
        }
        public override bool IsActive() => _framesHeld == 1;
    }
}