import * as cmn from './constants.bicep'

param env cmn.env
param system string
param tags cmn.tags = {
  Creator: cmn.creator
  Environment: env
}

resource sigr 'Microsoft.SignalRService/signalR@2025-01-01-preview' = {
  name: 'sigr-${system}-${env}-${cmn.companyInfo.name}'
  location: cmn.location
  tags: tags
}
