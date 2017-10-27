﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using TSettings.Encryptions;
using TSettings.Overrides;

namespace TSettings
{
    public class Settings
    {
        #region Getters and setters
        public static Settings Default
        {
            get { return _default ?? (_default = new Settings()); }
        }

        public object this[string key]
        {
            get { return Get<object>(key); }
            set { Set(key, value); }
        }

        public static IEncrpytion Encryption { get; set; }
        #endregion

        #region Private const
        private static Settings _default;
        private readonly string _filename;
        private SerializableDictionary<string, object> _settingsDictionary = new SerializableDictionary<string, object>();
        #endregion

        #region Contructors
        /// <summary>
        /// Create a instance of TSettings
        /// </summary>
        /// <param name="filename">Settings File Name</param>
        public Settings(string filename = "settings.bin")
        {
            _filename = filename;
            _default = this;

            if (File.Exists(_filename))
            {
                Load();
            }
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Create a instance of Default for Settings.Default
        /// </summary>
        /// <param name="filename">Settings File Name</param>
        /// <returns>returns Type of Settings</returns>
        public static Settings Init(string filename = "settings.bin")
        {
            var s = new Settings(filename);
            _default = s;

            return _default;
        }

        /// <summary>
        /// Create a instance of Default for Settings.Default using an IEncryption Type
        /// </summary>
        /// <param name="filename">Settings File name</param>
        /// <param name="encType">Encryption Type using IEncryption</param>
        /// <returns>returns Type of Settings</returns>
        public static Settings Init(string filename, IEncrpytion encType)
        {
            Encryption = encType;
            var s = new Settings(filename);
            _default = s;

            return _default;
        }
        #endregion

        #region Load/Save
        /// <summary>
        /// Save the settings to a file
        /// </summary>
        public void Save()
        {
            if (!string.IsNullOrWhiteSpace(Path.GetDirectoryName(_filename)) && !Directory.Exists(Path.GetDirectoryName(_filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(_filename));

            using (var fileStream = File.Create(_filename))
            {
                if (Encryption == null)
                {
                    IFormatter bf = new BinaryFormatter();
                    bf.Serialize(fileStream, _settingsDictionary);
                }
                else
                {
                    using (var enc = new CryptoStream(fileStream, Encryption.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        var bf = new BinaryFormatter();
                        bf.Serialize(enc, _settingsDictionary);
                    }
                }
            }
        }

        private void Load()
        {
            try
            {
                using (var fileStream = File.Open(_filename, FileMode.Open))
                {
                    if (Encryption == null)
                    {
                        IFormatter bf = new BinaryFormatter();
                        _settingsDictionary = (SerializableDictionary<string, object>) bf.Deserialize(fileStream);
                    }
                    else
                    {
                        using (var enc = new CryptoStream(fileStream, Encryption.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            var bf = new BinaryFormatter();
                            _settingsDictionary = (SerializableDictionary<string, object>)bf.Deserialize(enc);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[Settings Error] {0}", e.ToString());
            }

        }
        #endregion

        #region Get Keys
        /// <summary>
        /// Gets a object from settings using Genaric type
        /// </summary>
        /// <typeparam name="T">Genaric Type of class</typeparam>
        /// <param name="key">Name of the setting</param>
        /// <param name="default">Default value Not Null</param>
        /// <returns>Returns genaric type of T</returns>
        public T Get<T>(string key, T @default)
        {
            if (!Exist(key)) return @default;
            if (_settingsDictionary[key] != null)
            {
                return (T)_settingsDictionary[key];
            }

            return @default;
        }

        /// <summary>
        /// Gets a object from settings using Genaric type
        /// </summary>
        /// <typeparam name="T">Genaric Type of class</typeparam>
        /// <param name="key">Name of the setting</param>
        /// <returns>Returns Genairc type of the given key (Throws KeyNotFound if doesn't exist)</returns>
        public T Get<T>(string key)
        {
            if (!Exist(key))
            {
                throw new KeyNotFoundException();
            }

            return (T)_settingsDictionary[key];
        }
        #endregion

        #region Set Keys
        /// <summary>
        /// Set/Update a value in the settings
        /// </summary>
        /// <typeparam name="T">Genaric type to save</typeparam>
        /// <param name="key">Name of the setting</param>
        /// <param name="value">The value to save</param>
        public void Set<T>(string key, T @value)
        {
            if (Exist(key) && _settingsDictionary[key] != null)
            {
                var test = _settingsDictionary[key].GetType();

                if (@value.GetType() == test)
                {
                    _settingsDictionary[key] = @value;
                }

                return;
            }

            _settingsDictionary.Add(key, @value);
        }
        #endregion

        #region Helper Function
        /// <summary>
        /// Returns the type of the given setting value
        /// </summary>
        /// <param name="key">Name of setting</param>
        /// <returns>Returns Type</returns>
        public Type GetValueType(string key)
        {
            return Exist(key) ? _settingsDictionary[key].GetType() : null;
        }

        /// <summary>
        /// Deletes a key from the Settings
        /// </summary>
        /// <param name="key">Name of setting</param>
        /// <returns>Boolean</returns>
        public bool Delete(string key)
        {
            if (!_settingsDictionary.ContainsKey(key)) return false;

            _settingsDictionary.Remove(key);

            return true;
        }

        /// <summary>
        /// Checks if a key exist
        /// </summary>
        /// <param name="key">Name of setting</param>
        /// <returns>Boolean</returns>
        public bool Exist(string key)
        {
            return _settingsDictionary.ContainsKey(key);
        }
        #endregion
    }
}
