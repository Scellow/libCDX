using System;
using System.Collections.Generic;

namespace CDX
{
    public interface IPreferences
    {
        IPreferences putBoolean(string key, bool val);

        IPreferences putInteger(string key, int val);

        IPreferences putLong(string key, long val);

        IPreferences putFloat(string key, float val);

        IPreferences putString(string key, string val);

        IPreferences put(Dictionary<string, object> vals);

        bool getBoolean(string key);

        int getInteger(string key);

        long getLong(string key);

        float getFloat(string key);

        string getString(string key);

        bool getBoolean(string key, bool defValue);

        int getInteger(string key, int defValue);

        long getLong(string key, long defValue);

        float getFloat(string key, float defValue);

        string getString(string key, string defValue);

        Dictionary<string, object> get();

        bool contains(string key);

        void clear();

        void remove(string key);

        void flush();
    }
}