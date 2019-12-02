mergeInto(LibraryManager.library, {
  
  initialize: function () {
    var vars = {};
    var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function(m,key,value) {
        vars[key] = value;
    });
    
    unityInstance.SendMessage("Molecule", "recreateMolecule", vars["save"]);
  }
});