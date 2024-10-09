// Confidential Information of Motphys. Not for disclosure or distribution without Motphys's prior
// written consent.
//
// This software contains code, techniques and know-how which is confidential and proprietary to
// Motphys.
//
// Product and Trade Secret source code contains trade secrets of Motphys.
//
// Copyright (C) 2020-2024 Motphys Technology Co., Ltd. All Rights Reserved.
//
// This software belongs to the Intellectual Property of Motphys. Use of this software is subject to
// the terms and conditions in the license file accompanying. You may not use this software except
// in compliance with the license file.

namespace Motphys.Rigidbody
{

    internal class ExpandableArray<T>
    {
        private T[] _values;
        private int _len;

        public ExpandableArray() : this(0) { }

        public ExpandableArray(int capacity)
        {
            _values = new T[capacity];
            _len = 0;
        }

        public void Reserve(int capacity)
        {
            if (_values.Length >= capacity)
            {
                return;
            }

            var buffer = new T[capacity];
            System.Array.Copy(_values, buffer, _len);
            _values = buffer;
        }

        public void Push(T value)
        {
            ExpandIfRequired();
            _values[_len++] = value;
        }

        public void Clear()
        {
            _len = 0;
        }

        public int length
        {
            get { return _len; }
        }

        public T[] rawArray
        {
            get { return _values; }
        }

        public T this[int index]
        {
            get
            {
                if (index >= _len)
                {
                    throw new System.IndexOutOfRangeException();
                }
                else
                {
                    return _values[index];
                }
            }
            set
            {
                if (index >= _len)
                {
                    throw new System.IndexOutOfRangeException();
                }
                else
                {
                    _values[index] = value;
                }
            }
        }

        private void ExpandIfRequired()
        {
            if (_len == _values.Length)
            {
                int newCapacity;
                if (_values.Length < 10)
                {
                    newCapacity = 10;
                }
                else
                {
                    newCapacity = _values.Length + _values.Length / 2;
                }

                Reserve(newCapacity);
            }
        }
    }
}
