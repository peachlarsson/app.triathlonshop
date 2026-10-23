import * as con from './constants.bicep'

param env con.env
param system string
param tags con.tags = {
  Creator: con.creator
  Environment: env
}

resource sigr 'Microsoft.SignalRService/signalR@2025-01-01-preview' = {
  name: 'sigr-${system}-${env}-${con.companyInfo.name}'
  location: con.location
  tags: tags
}
