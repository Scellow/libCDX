using System;

namespace CDX.GLFWBackend
{
    public interface IWindowListener {

        void created(Window window);

        void iconified(bool isIconified);
	
        void maximized(bool isMaximized);

        void focusLost();
	
        void focusGained();		
	
        bool closeRequested();
	
        void filesDropped(string[] files);

        void refreshRequested();

    }
}