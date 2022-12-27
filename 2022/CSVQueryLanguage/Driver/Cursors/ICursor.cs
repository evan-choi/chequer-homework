using System;

namespace CSVQueryLanguage.Driver.Cursors;

public interface ICursor : ICursorRecord, IDisposable
{
    bool Read();
}
