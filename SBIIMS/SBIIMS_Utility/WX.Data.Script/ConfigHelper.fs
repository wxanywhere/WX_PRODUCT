
module Config

let GetMyConfig() =
  let config  = 
    #if COMPILED 
      ConfigurationManager.GetSection("MyConfig") :?> MyConfig
    #else                        
      let path = __SOURCE_DIRECTORY__ + "/app.config"
      let fileMap = ConfigurationFileMap(path) 
      let config = ConfigurationManager.OpenMappedMachineConfiguration(fileMap) 
      config.GetSection("MyConfig") :?> MyConfig
    #endif

