import * as con from './constants.bicep'

param env con.env
param system string
param tags con.tags = {
  Creator: con.creator
  Environment: env
}

var resourceAbbr = 'appi'

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${resourceAbbr}-${system}-${env}-${con.companyInfo.name}'
  location: con.location
  tags: tags
  kind: 'web'
}

output appInsightsConnectionString string = appInsights.properties.ConnectionString
