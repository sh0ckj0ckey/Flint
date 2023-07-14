#include "pch.h"

#include <msclr/marshal.h>
#include <msclr/marshal_cppstd.h>
#include <functional>
#include "keyboard_layout.h"


using namespace System;
using namespace System::Runtime::InteropServices;
using System::Collections::Generic::List;

// https://learn.microsoft.com/cpp/dotnet/how-to-wrap-native-class-for-use-by-csharp?view=vs-2019
namespace interop
{
    public
    ref class LayoutMapManaged
    {
    public:
        LayoutMapManaged() :
            _map(new LayoutMap) {}

        ~LayoutMapManaged()
        {
            delete _map;
        }

        String^ GetKeyName(DWORD key) {
            return gcnew String(_map->GetKeyName(key).c_str());
        }

        void Updatelayout()
        {
            _map->UpdateLayout();
        }

    protected:
        !LayoutMapManaged()
        {
            delete _map;
        }

    private:
        LayoutMap* _map;
    };
}
