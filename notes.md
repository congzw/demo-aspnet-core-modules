# common question and answer

##  AmbiguousMatchException: same controller action name

    //add[Area("Default")] in controllers of area
    app.UseMvc(routes =>
    {
        routes.MapRoute(
            name: "areas",
            template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
    });

## todo
