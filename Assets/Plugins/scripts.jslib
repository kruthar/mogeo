mergeInto(LibraryManager.library, {
  
  showSaveURL: function (str) {
    $("#dialog").html("<p>"+Pointer_stringify(str)+"</p>");
    $("#dialog").dialog("open");
  },
  
  initialize: function () {
    var vars = {};
    var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function(m,key,value) {
        vars[key] = value;
    });
    
    unityInstance.SendMessage("Molecule", "recreateMolecule", vars["save"]);
  }
});