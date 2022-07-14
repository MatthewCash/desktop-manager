# taskbar-style

A simple program to adjust the Windows taskbar background and size.

The primary taskbar can only have its background changed, while secondary taskbars can be repositioned.

The taskbar style will reset to default in certain events (such as start menu opening), as a work around two events are registered to reapply taskbar style, on `EVENT_OBJECT_LOCATIONCHANGE` and `EVENT_OBJECT_FOCUS`
Other taskbar styling programs restyle the taskbar on a loop, which can look better but is likely less performant.