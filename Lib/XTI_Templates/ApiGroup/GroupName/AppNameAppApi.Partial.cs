using XTI___APPNAME____APPTYPE__Api.__GROUPNAME__;

namespace XTI___APPNAME____APPTYPE__Api;

partial class __APPNAME__AppApi
{
    private __GROUPNAME__Group? ___GROUPNAME__;

    public __GROUPNAME__Group __GROUPNAME__ { get => ___GROUPNAME__ ?? throw new ArgumentNullException(nameof(___GROUPNAME__)); }

    partial void create__GROUPNAME__Group(IServiceProvider sp)
    {
        ___GROUPNAME__ = new __GROUPNAME__Group
        (
            source.AddGroup(nameof(__GROUPNAME__)),
            sp
        );
    }
}