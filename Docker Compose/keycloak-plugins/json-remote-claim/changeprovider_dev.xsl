<?xml version="1.0" encoding="UTF-8"?>
<!-- To register our own module, which contains the protocol mapper, it is not enough to just
add the jar file to the jboss server. We have to change the jboss xml file too. This is done
by this xslt file -->
<xsl:stylesheet version="2.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="//ds:subsystem/ds:providers" xmlns:ds="urn:jboss:domain:keycloak-server:1.1">
        <ds:providers>
            <!-- We add our own module, which contains the protocol mapper, to the providers here.
             Without that, we would be unable to configure keycloak to use our protocol mapper. It wouldn't
             be visible in the list of available protocol mappers. The name of the module must be the
             same as in the module.xml -->
            <ds:provider>module:fr.sii.keycloak.mapper.json-remote-claim</ds:provider>
            <ds:provider>
                classpath:${jboss.home.dir}/providers/*
            </ds:provider>
        </ds:providers>
    </xsl:template>
	
	<xsl:template match="//ds:subsystem/ds:spi[@name='connectionsHttpClient']/ds:provider[@name='default']" xmlns:ds="urn:jboss:domain:keycloak-server:1.1">
         
            <ds:provider name="default" enabled="true">
				<ds:properties>
					<ds:property
					name="proxy-mappings" value="[&quot;.*;http://devproxy.incadeadev.loc:8080&quot;]"/>
				</ds:properties>
              </ds:provider>    
    </xsl:template>

    <xsl:template match="@*|node()">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
        </xsl:copy>
    </xsl:template>

</xsl:stylesheet>
