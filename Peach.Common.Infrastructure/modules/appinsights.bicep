import * as cmn from './constants.bicep'

param env cmn.env
param system string

var resourceAbbr = 'appi'

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${resourceAbbr}-${system}-${env}-${cmn.companyInfo.name}'
  location: cmn.location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    RetentionInDays: 90
  }
}

output appInsightsConnectionString string = appInsights.properties.ConnectionString

