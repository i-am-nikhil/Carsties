import NextAuth, { Profile } from "next-auth"
import { OIDCConfig } from "next-auth/providers"
import DuendeIDS6Provider from "next-auth/providers/duende-identity-server6"

export const { handlers, signIn, signOut, auth } = NextAuth({
  providers: [
    DuendeIDS6Provider({
    id: 'id-server',
    clientId: "nextApp", // Matches clientId from Config.cs in IdentityServer
    clientSecret: "secret", //Matches ClientSecrets from Config.cs in IdentityServer. This file stays on the server so its safe to keep secrets here.
    issuer: "http://localhost:5000", // Matches IdentityServer URL
    authorization: {params: {scope: 'openid profile auctionApp'}},
    idToken: true // Set to true to get id_token
  } as OIDCConfig<Omit<Profile, 'username'>>),
  ],
  pages: {
    signIn: '/signin'
  },
  callbacks: {
    async authorized({auth}){
      return !!auth // returns a boolean indicating auth object is populated.
    },
    async jwt({ token, profile, account }) {
      if (profile) {
        token.username = profile.username;
      }
      if (account && account.access_token){
        token.accessToken = account.access_token; 
      }
      return token;
    },
    async session({ session, token }) {
      if (token) {
        session.user.username = token.username;
        session.accessToken = token.accessToken;
      }
      return session;
    }
  }
})