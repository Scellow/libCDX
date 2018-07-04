using System;
using System.Text.RegularExpressions;

namespace CDX.Graphics.GLUtils
{
    public class GLVersion
    {
        private int majorVersion;
        private int minorVersion;
        private int releaseVersion;

        private readonly string vendorString;
        private readonly string rendererString;

        private readonly Type type;

        private readonly string TAG = "GLVersion";

        public GLVersion(ApplicationType appType, string versionString, string vendorString, string rendererString)
        {
            if (appType == ApplicationType.Android) this.type      = Type.GLES;
            else if (appType == ApplicationType.iOS) this.type     = Type.GLES;
            else if (appType == ApplicationType.Desktop) this.type = Type.OpenGL;
            else if (appType == ApplicationType.Applet) this.type  = Type.OpenGL;
            else if (appType == ApplicationType.WebGL) this.type   = Type.WebGL;
            else this.type                                         = Type.NONE;


            this.vendorString   = vendorString;
            this.rendererString = rendererString;

            var versionData = versionString.Split(' ')[0].Split('.');
            majorVersion = Convert.ToInt32(versionData[0]);
            minorVersion = Convert.ToInt32(versionData[1]);


            return;
            if (type == Type.GLES)
            {
                //OpenGL<space>ES<space><version number><space><vendor-specific information>.
                ExtractVersion("OpenGL ES (\\d(\\.\\d){0,2})", versionString);
            }
            else if (type == Type.WebGL)
            {
                //WebGL<space><version number><space><vendor-specific information>
                ExtractVersion("WebGL (\\d(\\.\\d){0,2})", versionString);
            }
            else if (type == Type.OpenGL)
            {
                //<version number><space><vendor-specific information>
                ExtractVersion("(\\d(\\.\\d){0,2})", versionString);
            }
            else
            {
                majorVersion   = -1;
                minorVersion   = -1;
                releaseVersion = -1;
                vendorString   = "";
                rendererString = "";
            }

            this.vendorString   = vendorString;
            this.rendererString = rendererString;
        }

        private void ExtractVersion(string patternString, string versionString)
        {
            var pattern = new Regex(patternString);
            var matcher = pattern.Match(versionString);
            var  found   = matcher.Success;
            if (found)
            {
                var   result      = matcher.Groups[1].Value;
                var resultSplit = result.Split(Convert.ToChar("\\."));
                majorVersion   = parseInt(resultSplit[0], 2);
                minorVersion   = resultSplit.Length < 2 ? 0 : parseInt(resultSplit[1], 0);
                releaseVersion = resultSplit.Length < 3 ? 0 : parseInt(resultSplit[2], 0);
            }
            else
            {
                Gdx.app.log(TAG, "Invalid version string: " + versionString);
                majorVersion   = 2;
                minorVersion   = 0;
                releaseVersion = 0;
            }
        }

        private int parseInt(string v, int defaultValue)
        {
            try
            {
                return int.Parse(v);
            }
            catch (Exception nfe)
            {
                Gdx.app.error("LibGDX GL", "Error parsing number: " + v + ", assuming: " + defaultValue);
                return defaultValue;
            }
        }

        public Type getType()
        {
            return type;
        }

        public int getMajorVersion()
        {
            return majorVersion;
        }

        public int getMinorVersion()
        {
            return minorVersion;
        }

        public int getReleaseVersion()
        {
            return releaseVersion;
        }

        public string getVendorString()
        {
            return vendorString;
        }

        public string getRendererString()
        {
            return rendererString;
        }

        public bool isVersionEqualToOrHigher(int testMajorVersion, int testMinorVersion)
        {
            return majorVersion > testMajorVersion || (majorVersion == testMajorVersion && minorVersion >= testMinorVersion);
        }

        public string getDebugVersionString()
        {
            return "Type: " + type + "\n" +
                   "Version: " + majorVersion + ":" + minorVersion + ":" + releaseVersion + "\n" +
                   "Vendor: " + vendorString + "\n" +
                   "Renderer: " + rendererString;
        }

        public enum Type
        {
            OpenGL,
            GLES,
            WebGL,
            NONE
        }
    }
}